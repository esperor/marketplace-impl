using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using course.Server.Data;
using course.Server.Services;
using course.Server.Configs;
using course.Server.Configs.Enums;
using course.Server.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using course.Server.Models.Identity;

namespace course.Server.Controllers.Client
{
    [Route("api/client/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityService _identityService;

        public OrderController(ApplicationDbContext context,
            IdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        // GET: api/client/order
        [HttpGet]
        [AuthorizeAccessLevel(EAccessLevel.Client)]
        public async Task<ActionResult<IEnumerable<OrderUserInfoModel>>> GetOrders(
            int offset = 0,
            int limit = 10)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();


            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .GroupJoin(
                    _context.OrderRecords,
                    o => o.Id,
                    r => r.OrderId,
                    (order, records) => new { order, records })
                .ToListAsync();

            IQueryable<OrderUserInfoModel> set = Enumerable.Empty<OrderUserInfoModel>().AsQueryable();

            foreach (var item in orders)
            {
                Dictionary<InventoryRecord, int> iRecords = [];
                foreach (var orderRecord in item.records)
                {
                    var ir = await _context.InventoryRecords.FindAsync(orderRecord.InventoryRecordId);
                    iRecords.Add(ir!, orderRecord.Quantity);
                }
                set = set.Append(new OrderUserInfoModel(item.order, item.records.ToList()));
            }
            return set.Skip(offset).Take(limit).ToList();
        }


        // GET: api/client/order/5
        [HttpGet("{id}")]
        [AuthorizeAccessLevel(EAccessLevel.Client)]
        public async Task<ActionResult<OrderUserInfoModel>> GetOrder(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            var result = CheckUserForOrder(user, id, out var order);
            if (result != null) return result;

            var orderRecords = await _context.OrderRecords
                .Where(r => r.OrderId == id)
                .ToListAsync();

            return new OrderUserInfoModel(order!, orderRecords);
        }

        // POST: api/client/order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AuthorizeAccessLevel(EAccessLevel.Client)]
        public async Task<ActionResult<OrderUserInfoModel>> PostOrder(OrderPostModel model)
        {
            if (model.OrderedRecords.Count == 0) return BadRequest();

            _context.Database.BeginTransaction();
            EntityEntry<Order> entry;
            try
            {
                model.UserId ??= (await _identityService.GetUser(HttpContext))!.Id;

                entry = _context.Orders.Add(model.ToEntity());
                await _context.SaveChangesAsync();

                try
                {
                    var orderRecords = model.OrderedRecords.Select((recordInfo) => {
                        var id = recordInfo.Key;
                        if (!_context.InventoryRecords.Any(r => r.Id == id))
                            throw new ArgumentException("No such inventory record");
                        var quantity = recordInfo.Value;
                        return new OrderRecord
                        {
                            OrderId = entry.Entity.Id,
                            InventoryRecordId = id,
                            Quantity = quantity,
                            Status = EOrderRecordStatus.Created
                        };
                    });

                    _context.OrderRecords.AddRange(orderRecords);
                } catch (ArgumentException)
                {
                    return NotFound();
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();

            } catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }

            return Ok();
        }

        [HttpPut]
        [Route("{id}/cancel")]
        [AuthorizeAccessLevel(EAccessLevel.Client)]
        public async Task<ActionResult> CancelOrder([FromRoute] int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            var result = CheckUserForOrder(user, id, out var order);
            if (result != null) return result;

            var orderRecords = await _context.OrderRecords
                .Where(r => r.OrderId == id)
                .ToListAsync();

            foreach (var record in orderRecords)
            {
                record.Status = EOrderRecordStatus.Canceled;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private ActionResult? CheckUserForOrder(
            ApplicationUserExtended? user,
            int orderId,
            out Order? order)
        {
            order = _context.Orders.Find(orderId);

            if (user is null) return BadRequest();
            if (order is null) return NotFound();

            if (user.GetAccessLevel() < EAccessLevel.Administrator &&
                user.Id != order.UserId) return Forbid();

            return null;
        }
    }
}
