using marketplace_impl.Server.Data;

namespace marketplace_impl.Server.Models
{
    public class InventoryRecordInfoModel
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? PropertiesJson { get; set; }

        public string? Size { get; set; }

        public string Variation { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

        public byte[]? Image { get; set; }

        public double? Rating { get; set; }

        public InventoryRecordInfoModel() { }

        public InventoryRecordInfoModel(ProductRecordDbModel dbModel)
        {
            Id = (int)dbModel.RecordId!;
            Price = (int)dbModel.Price!;
            Image = dbModel.Image;
            Quantity = (int)dbModel.Quantity!;
            PropertiesJson = dbModel.PropertiesJson;
            Size = dbModel.Size;
            Variation = dbModel.Variation!;
            Rating = dbModel.Rating;
        }

        public InventoryRecordInfoModel(InventoryRecord inventoryRecord)
        {
            Id = inventoryRecord.Id;
            PropertiesJson = inventoryRecord.PropertiesJson;
            Variation = inventoryRecord.Variation;
            Size = inventoryRecord.Size;
            Quantity = inventoryRecord.Quantity;
            Price = inventoryRecord.Price;
            Image = inventoryRecord.Image;
            Title = inventoryRecord.Product?.Title;
        }
    }
}
