using marketplace_impl.Server.Data;

namespace marketplace_impl.Server.Models
{
    public class OrderBaseAggregatedInfoModel
    {
        public int Id { get; set; }
        public int TotalPrice { get; set; }
        public DateOnly Date { get; set; }
        public List<OrderRecordInfoModel> OrderRecords { get; set; }

        public OrderBaseAggregatedInfoModel() { }

        public OrderBaseAggregatedInfoModel(Order order, List<OrderRecord> records)
        {
            Id = order.Id;
            Date = order.Date;

            var totalPriceAgg = 0;

            OrderRecords = records.Select(record => {
                totalPriceAgg += record.InventoryRecord.Price * record.Quantity;
                return new OrderRecordInfoModel
                {
                    Id = record.Id,
                    OrderId = record.OrderId,
                    InventoryRecordId = record.InventoryRecordId,
                    Quantity = record.Quantity,
                    Status = record.Status,
                    ProductTitle = record.InventoryRecord?.Product?.Title ?? "",
                    ProductVariation = record.InventoryRecord?.Variation ?? "",
                    RatingComment = record.RatingComment,
                    RatingDate = record.RatingDate,
                    RatingValue = record.RatingValue,
                };
            }).ToList();

            TotalPrice = totalPriceAgg;
        }

        public OrderBaseAggregatedInfoModel(List<OrderRecordDbModel> dbModels)
        {
            var firstModel = dbModels.FirstOrDefault();
            if (firstModel is null) throw new Exception("Argument empty");

            Id = firstModel.OrderId;
            Date = DateOnly.FromDateTime(firstModel.Date);

            var totalPriceAgg = 0;

            OrderRecords = dbModels.Select(m => {
                totalPriceAgg += m.Price * m.Quantity;
                return new OrderRecordInfoModel
                {
                    Id = m.Id,
                    OrderId = m.OrderId,
                    InventoryRecordId = m.InventoryRecordId,
                    Quantity = m.Quantity,
                    Status = m.Status,
                    ProductTitle = m.ProductTitle,
                    ProductVariation = m.ProductVariation,
                    RatingComment = m.RatingComment,
                    RatingValue = m.RatingValue,
                    RatingDate = m.RatingDate,
                };
            }).ToList();

            TotalPrice = totalPriceAgg;
        }
    }
}
