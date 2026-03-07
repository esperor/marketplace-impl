using course.Server.Data;

namespace course.Server.Models
{
    public class OrderBaseInfoModel
    {
        public int Id { get; set; }
        public int TotalPrice { get; set; }
        public DateOnly Date { get; set; }
        public List<OrderRecordInfoModel> OrderRecords { get; set; }

        public OrderBaseInfoModel() { }

        public OrderBaseInfoModel(Order order, List<OrderRecord> records)
        {
            Id = order.Id;
            Date = order.Date;

            var totalPriceAgg = 0;

            OrderRecords = records.Select(record => {
                totalPriceAgg += record.Record.Price * record.Quantity;
                return new OrderRecordInfoModel
                {
                    Id = record.Id,
                    OrderId = record.OrderId,
                    InventoryRecordId = record.InventoryRecordId,
                    Quantity = record.Quantity,
                    Status = record.Status
                };
            }).ToList();

            TotalPrice = totalPriceAgg;
        }
    }
}
