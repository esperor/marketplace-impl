using marketplace_impl.Server.Configs.Enums;

namespace marketplace_impl.Server.Models
{
    public class OrderRecordInfoModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int InventoryRecordId { get; set; }
        public int Quantity { get; set; }
        public EOrderRecordStatus Status { get; set; }
        public string ProductTitle { get; set; }
        public string ProductVariation { get; set; }
        public int? RatingValue { get; set; }
        public string? RatingComment { get; set; }
        public DateOnly? RatingDate { get; set; }
    }
}
