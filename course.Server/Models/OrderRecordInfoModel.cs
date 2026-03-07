using course.Server.Configs.Enums;
using System.Composition.Convention;

namespace course.Server.Models
{
    public class OrderRecordInfoModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int InventoryRecordId { get; set; }
        public int Quantity { get; set; }
        public EOrderRecordStatus Status { get; set; }
    }
}
