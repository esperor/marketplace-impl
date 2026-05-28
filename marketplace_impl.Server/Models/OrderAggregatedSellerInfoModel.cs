namespace course.Server.Models
{
    public class OrderAggregatedSellerInfoModel : OrderBaseAggregatedInfoModel
    {
        public string? DelivererName { get; set; }
        public string? DelivererContactInfo { get; set; }

        public OrderAggregatedSellerInfoModel() { }

        public OrderAggregatedSellerInfoModel(List<OrderRecordDbModel> orderRecordModels)
        {
            var firstRecord = orderRecordModels.FirstOrDefault()
                ?? throw new Exception("Passed records list is empty");

            Id = firstRecord.OrderId;
            Date = DateOnly.FromDateTime(firstRecord.Date);
            DelivererName = firstRecord.DelivererName;
            DelivererContactInfo = firstRecord.DelivererContactInfo;

            var totalPriceAgg = 0;

            OrderRecords = orderRecordModels.Select(model => {
                totalPriceAgg += model.Price * model.Quantity;
                return new OrderRecordInfoModel
                {
                    Id = model.Id,
                    OrderId = model.OrderId,
                    InventoryRecordId = model.InventoryRecordId,
                    Quantity = model.Quantity,
                    Status = model.Status,
                    ProductTitle = model.ProductTitle,
                    ProductVariation = model.ProductVariation
                };
            }).ToList();

            TotalPrice = totalPriceAgg;
        }
    }
}
