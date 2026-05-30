namespace marketplace_impl.Server.Models
{
    public class ProductRecordDbModel
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? RecordId { get; set; }
        public int? Quantity { get; set; }
        public int? Price { get; set; }
        public byte[]? Image { get; set; }
        public string? PropertiesJson { get; set; }
        public string? Size { get; set; }
        public string? Variation { get; set; }
        public double? Rating { get; set; }
    }
}
