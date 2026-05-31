using marketplace_impl.Server.Configs.Authorization;
using marketplace_impl.Server.Configs.Enums;
using marketplace_impl.Server.Data;
using marketplace_impl.Server.Models;
using marketplace_impl.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketplace_impl.Server.Controllers.Business
{
    [Route("api/business/inventory-record")]
    [ApiController]
    [AuthorizeAccessTrait(EAccessTrait.Seller)]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityService _identityService;

        public InventoryController(ApplicationDbContext context, IdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        // GET: api/business/inventory-record/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryRecordInfoModel>> GetInventoryRecord(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var record = await _context.InventoryRecords.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (record is null) return NotFound();

            return new InventoryRecordInfoModel(record);
        }

        // DELETE: api/business/inventory-record/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryRecord(int id)
        {
            var user = await _identityService.GetUser(HttpContext);
            if (user is null) return BadRequest();

            var inventoryRecord = await _context.InventoryRecords
                .Where(irec => irec.Id == id)
                .Include(irec => irec.Product.Store)
                .FirstOrDefaultAsync();

            if (inventoryRecord == null) return NotFound();

            if (inventoryRecord.Product.Store.OwnerId != user.Id)
            {
                return Unauthorized("Permission denied");
            }    

            _context.InventoryRecords.Remove(inventoryRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/inventory-record/product/1
        // Both inserts new records and updates existing ones
        [HttpPost]
        [Route("product/{productId}")]
        public async Task<IActionResult> PostInventoryRecords(
            [FromRoute] int productId, 
            [FromBody] InventoryRecordInputModel[] models)
        {
            throw new NotImplementedException();
            var product = await _context.Products.FindAsync(productId);
            if (product is null)
                return NotFound();

            IEnumerable<InventoryRecord> entities;
            try
            {
                entities = models.Select(m => {
                    if (m.ProductId != productId)
                        throw new ArgumentException("Record's productId doesn't match provided productId");
                    var entity = m.ToEntity();
                    entity.Product = product;
                    return entity;
                });
            } catch (ArgumentException)
            {
                return BadRequest();
            }

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = 
                    entity.Id != 0 ? EntityState.Modified : EntityState.Added;
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
