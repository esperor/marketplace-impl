using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using marketplace_impl.Server.Data;
using marketplace_impl.Server.Services;
using marketplace_impl.Server.Configs;
using marketplace_impl.Server.Configs.Enums;
using marketplace_impl.Server.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using marketplace_impl.Server.Models.Identity;
using Microsoft.EntityFrameworkCore.Internal;

namespace marketplace_impl.Server.Controllers.Client
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
        [AuthorizeAccessTrait(EAccessTrait.Client)]
        public async Task<ActionResult<IEnumerable<OrderUserAggregatedInfoModel>>> GetOrders(
            int offset = 0,
            int limit = 100)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordDbModel>(
               $"select * from FN_GetOrderRecords({null}, {null}, {user.Id}, {null}, {null}, {offset}, {limit})").ToListAsync();

            List<OrderUserAggregatedInfoModel> set = [];

            foreach (var group in sqlResult.GroupBy(dbModel => dbModel.OrderId))
            {
                set.Add(new OrderUserAggregatedInfoModel(group.ToList()));
            }
            return set;
        }


        // GET: api/client/order/5
        [HttpGet("{id}")]
        [AuthorizeAccessTrait(EAccessTrait.Client)]
        public async Task<ActionResult<OrderUserAggregatedInfoModel>> GetOrder(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordDbModel>(
                $"select * from FN_GetOrderRecords({null}, {null}, {user.Id}, {null}, {id}, {null}, {null})").ToListAsync();

            return new OrderUserAggregatedInfoModel(sqlResult);
        }

        // POST: api/client/order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AuthorizeAccessTrait(EAccessTrait.Client)]
        public async Task<ActionResult<OrderUserAggregatedInfoModel>> PostOrder(OrderPostModel model)
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

        // PUT: api/client/order/rate-record
        [HttpPut("rate-record")]
        [AuthorizeAccessTrait(EAccessTrait.Client)]
        public async Task<ActionResult> RateOrderRecord(OrderRecordRatingPutModel model)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var result = await _context.OrderRecords
                .Where(or => or.Id == model.Id)
                .Include(or => or.Order)
                .FirstOrDefaultAsync();
            if (result is null) return NotFound();

            if (result.Order.UserId != user.Id) return BadRequest();

            result.RatingValue = model.RatingValue;
            result.RatingComment = model.RatingComment;
            result.RatingDate = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("{id}/cancel")]
        [AuthorizeAccessTrait(EAccessTrait.Client)]
        public async Task<ActionResult> CancelOrder([FromRoute] int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            var result = TryGetUserOrderById(user, id, out var order);
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

        private ActionResult? TryGetUserOrderById(
            ApplicationUserExtended? user,
            int orderId,
            out Order? order)
        {
            order = _context.Orders.Find(orderId);

            if (user is null) return BadRequest();
            if (order is null) return NotFound();

            if (user.Id != order.UserId) return Forbid();

            return null;
        }
    }
}
