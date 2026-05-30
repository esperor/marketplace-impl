using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models
{
    public class ProductRecordInfoModel : ProductInfoModel
    {
        [Required]
        public InventoryRecordInfoModel? Record { get; set; }

        public ProductRecordInfoModel(ProductRecordDbModel dbModel)
        {
            Id = dbModel.ProductId;
            StoreId = dbModel.StoreId;
            StoreName = dbModel.StoreName;
            Title = dbModel.Title;
            Description = dbModel.Description;
            Record = (dbModel.Quantity ?? 0) <= 0
                ? null
                : new InventoryRecordInfoModel(dbModel);
        }
    }
}
