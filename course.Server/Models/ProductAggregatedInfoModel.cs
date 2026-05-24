using course.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace course.Server.Models
{
    public class ProductAggregatedInfoModel : ProductInfoModel
    {
        [Required]
        public InventoryRecordInfoModel[]? Records { get; set; }

        public ProductAggregatedInfoModel() { }

        public ProductAggregatedInfoModel(Product p)
        {
            Id = p.Id;
            StoreId = p.StoreId;
            StoreName = p.Store?.Name;
            Title = p.Title;
            Description = p.Description;
        }

        public ProductAggregatedInfoModel(IEnumerable<ProductRecordDbModel> dbModels)
        {
            var firstModel = dbModels.FirstOrDefault()
                ?? throw new ArgumentException("Argument empty");
            
            Id = firstModel.ProductId;
            StoreId = firstModel.StoreId;
            StoreName = firstModel.StoreName;
            Title = firstModel.Title;
            Description = firstModel.Description;
            Records = dbModels?
                .Where(r => r.RecordId is not null)
                .Select(r => new InventoryRecordInfoModel(r))
                .ToArray();
        }
    }
}
