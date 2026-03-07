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
            EOrderRecordStatus status,
            int offset = 0,
            int limit = 10)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordSellerDbModel>(
                $"select * from FN_GetOrders({user.Id}, {(int)status}, {storeId}, {null}, {offset}, {limit})").ToListAsync();

            return sqlResult;
        }

        // GET: api/business/order/5
        [HttpGet("{id}")]
        [AuthorizeAccessTrait(EAccessTrait.Seller)]
        public async Task<ActionResult<OrderAggregatedSellerInfoModel>> GetOrder(
            int id,
            int? storeId,
            EOrderRecordStatus status,
            int offset = 0,
            int limit = 10)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var sqlResult = await _context.Database.SqlQuery<OrderRecordSellerDbModel>(
                $"select * from FN_GetOrdersBySeller({user.Id}, {(int)status}, {storeId}, {id}, {offset}, {limit})").ToListAsync();

            return new OrderAggregatedSellerInfoModel(sqlResult);
        }
    }
}
