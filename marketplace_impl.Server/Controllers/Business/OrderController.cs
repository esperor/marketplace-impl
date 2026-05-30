using marketplace_impl.Server.Configs.Authorization;
using marketplace_impl.Server.Configs.Enums;
using marketplace_impl.Server.Data;
using marketplace_impl.Server.Models;
using marketplace_impl.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketplace_impl.Server.Controllers.Business
{
    [Route("api/business/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityService _identityService;
        private readonly BusinessService _businessService;

        public OrderController(
            ApplicationDbContext context,
            IdentityService identityService,
            BusinessService businessService)
        {
            _context = context;
            _identityService = identityService;
            _businessService = businessService;
        }

        // GET: api/business/order
        [HttpGet]
        [AuthorizeAccessTrait(EAccessTrait.Seller)]
        public async Task<ActionResult<List<OrderRecordDbModel>>> GetOrders(
            int? storeId,
            EOrderRecordStatus? status,
            int offset = 0,
            int limit = 10)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordDbModel>(
                $"select * from FN_GetOrderRecords({user.Id}, {(int?)status}, {null}, {storeId}, {null}, {offset}, {limit})").ToListAsync();

            return sqlResult;
        }

        // GET: api/business/order/5
        [HttpGet("{id}")]
        [AuthorizeAccessTrait(EAccessTrait.Seller)]
        public async Task<ActionResult<OrderAggregatedSellerInfoModel>> GetOrder(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordDbModel>(
                $"select * from FN_GetOrderRecords({user.Id}, {null}, {null}, {null}, {id}, {null}, {null})").ToListAsync();

            return new OrderAggregatedSellerInfoModel(sqlResult);
        }

        public record UpdateOrderRecordStatusRequestBody(EOrderRecordStatus NewStatus);

        // GET: api/business/order/record/5/status
        [HttpPut("record/{id}/status")]
        [AuthorizeAccessTrait(EAccessTrait.Seller)]
        public async Task<ActionResult> UpdateOrderRecordStatus(int id, [FromBody] UpdateOrderRecordStatusRequestBody body)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest("Нет доступа");

            if (!new List<EOrderRecordStatus>() { EOrderRecordStatus.Packaged, EOrderRecordStatus.Created }.Contains(body.NewStatus))
                return BadRequest("Некорретный статус");

            var orderRecord = await _context.OrderRecords
                .Where(orec => orec.Id == id)
                .Include(orec => orec.InventoryRecord)
                .Include(orec => orec.InventoryRecord.Product)
                .Include(orec => orec.InventoryRecord.Product.Store)
                .FirstOrDefaultAsync();
            if (orderRecord is null || orderRecord.InventoryRecord.Product.Store.OwnerId != user.Id) return NotFound("Целевая запись не найдена");

            if (!((orderRecord.Status == EOrderRecordStatus.Created && body.NewStatus == EOrderRecordStatus.Packaged) 
                || (orderRecord.Status == EOrderRecordStatus.Packaged && body.NewStatus == EOrderRecordStatus.Created)))
            {
                return BadRequest("Некорректный статус");
            }

            orderRecord.Status = body.NewStatus;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
