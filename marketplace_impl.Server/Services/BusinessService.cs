using course.Server.Data;
using course.Server.Models;

namespace course.Server.Services
{
    public class BusinessService(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public async Task UpdateProduct(Product product, ProductAggregatedInfoModel model)
        {
            product.Title = model.Title;
            product.Description = model.Description;

            await UpdateInventoryRecordsWithoutCommit(product.Id, model.Records ?? []);

            await _context.SaveChangesAsync();
        }

        private async Task UpdateInventoryRecordsWithoutCommit(
            int productId,
            InventoryRecordInfoModel[] models)
        {
            foreach (var model in models)
            {
                var inventoryRecord = await _context.InventoryRecords.FindAsync(model.Id);
                if (inventoryRecord is null)
                {
                    await _context.InventoryRecords.AddAsync(new InventoryRecordInputModel()
                    {
                        ProductId = productId,
                        Quantity = model.Quantity,
                        Price = model.Price,
                        Size = model.Size,
                        Variation = model.Variation,
                        PropertiesJson = model.PropertiesJson,
                        Image = model.Image,
                    }.ToEntity());

                    continue;
                }

                inventoryRecord.Size = model.Size;
                inventoryRecord.Variation = model.Variation;
                inventoryRecord.Price = model.Price;
                inventoryRecord.Quantity = model.Quantity;
                inventoryRecord.Image = model.Image;
                inventoryRecord.PropertiesJson = model.PropertiesJson;
            }
        }
    }
}
