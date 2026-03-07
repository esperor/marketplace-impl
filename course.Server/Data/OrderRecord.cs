using course.Server.Configs.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace course.Server.Data
{
    [Table("order_record")]
    public class OrderRecord
    {
        [Key]
        public int Id { get; set; }

        public required int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        public required int InventoryRecordId { get; set; }

        [ForeignKey(nameof(InventoryRecordId))]
        public InventoryRecord Record { get; set; }
        
        public required int Quantity { get; set; }

        public required EOrderRecordStatus Status { get; set; } = EOrderRecordStatus.Created;
    }
}
