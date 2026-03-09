using course.Server.Configs;
using course.Server.Configs.Enums;
using course.Server.Data;
using course.Server.Models;
using course.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace course.Server.Controllers.Business
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
        public async Task<ActionResult<List<OrderRecordSellerDbModel>>> GetOrders(
            int? storeId,
            EOrderRecordStatus? status,
            int offset = 0,
            int limit = 10)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordSellerDbModel>(
                $"select * from FN_GetOrders({user.Id}, {(int?)status}, {storeId}, {null}, {offset}, {limit})").ToListAsync();

            return sqlResult;
        }

        // GET: api/business/order/5
        [HttpGet("{id}")]
        [AuthorizeAccessTrait(EAccessTrait.Seller)]
        public async Task<ActionResult<OrderAggregatedSellerInfoModel>> GetOrder(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordSellerDbModel>(
                $"select * from FN_GetOrders({user.Id}, {null}, {null}, {id}, {null}, {null})").ToListAsync();

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
                .Include(orec => orec.Record)
                .Include(orec => orec.Record.Product)
                .Include(orec => orec.Record.Product.Store)
                .FirstOrDefaultAsync();
            if (orderRecord is null || orderRecord.Record.Product.Store.OwnerId != user.Id) return NotFound("Целевая запись не найдена");

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
