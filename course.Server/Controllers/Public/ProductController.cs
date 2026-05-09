using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using course.Server.Data;
using course.Server.Configs.Enums;
using course.Server.Models;

namespace course.Server.Controllers.Public
{
    [Route("api/public/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/public/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRecordInfoModel>>> GetProducts(
            string? searchString,
            int? storeId,
            [FromQuery] EProductOrdering orderBy = EProductOrdering.None,
            int offset = 0,
            int limit = 10)
        {

            var sqlResult = _context.Database.SqlQuery<ProductRecordDbModel>(
                $"select * from FN_GetProducts({searchString}, {storeId}, {orderBy}, {null}, {offset}, {limit})");

            return await sqlResult.Select(item => new ProductRecordInfoModel(item)).ToListAsync();
        }

        // GET: api/public/product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductAggregatedInfoModel>> GetProduct(int id)
        {
            var sqlResult = await _context.Database.SqlQuery<ProductRecordDbModel>(
                $"select * from FN_GetProducts({null}, {null}, {null}, {id}, {null}, {null})").ToListAsync();

            if (sqlResult.Count == 0)
            {
                return NotFound();
            }

            return new ProductAggregatedInfoModel(sqlResult);
        }
    }
}
