using marketplace_impl.Server.Configs.Enums;

namespace marketplace_impl.Server.Models
{
    public class OrderRecordDbModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int InventoryRecordId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductVariation { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public EOrderRecordStatus Status { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string? DelivererContactInfo { get; set; }
        public string? DelivererName { get; set; }
        public int? RatingValue { get; set; }
        public string? RatingComment { get; set; }
        public DateOnly? RatingDate { get; set; }
    }
}
