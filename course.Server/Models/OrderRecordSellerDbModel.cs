using course.Server.Configs.Enums;

namespace course.Server.Models
{
    public class OrderRecordSellerDbModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int InventoryRecordId { get; set; }
        public DateTime Date { get; set; }
        public EOrderRecordStatus Status { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string? DelivererContactInfo { get; set; }
        public string? DelivererName { get; set; }
    }
}
