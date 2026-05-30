using marketplace_impl.Server.Data;

namespace marketplace_impl.Server.Models
{
    public class OrderUserAggregatedInfoModel : OrderBaseAggregatedInfoModel
    {
        public int UserId { get; set; }
        public string Address { get; set; }

        public OrderUserAggregatedInfoModel() { }

        public OrderUserAggregatedInfoModel(Order order, List<OrderRecord> records)
            : base(order, records)
        {
            UserId = order.UserId;
            Address = order.Address;
        }

        public OrderUserAggregatedInfoModel(List<OrderRecordDbModel> dbModels)
            : base(dbModels)
        {
            var firstModel = dbModels.FirstOrDefault();
            if (firstModel is null) throw new Exception("Argument empty");

            UserId = firstModel.UserId;
            Address = firstModel.Address;
        }
    }
}
