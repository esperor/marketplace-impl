using marketplace_impl.Server.Data;

namespace marketplace_impl.Server.Models
{
    public class OrderPostModel
    {
        public int? UserId { get; set; }

        public string Address { get; set; }

        // key is record.Id and value is quantity
        public Dictionary<int, int> OrderedRecords { get; set; }

        public Order ToEntity()
        {
            var order = new Order
            {
                Address = Address,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            if (UserId != null) order.UserId = UserId.Value;
            return order;
        }
    }
}
