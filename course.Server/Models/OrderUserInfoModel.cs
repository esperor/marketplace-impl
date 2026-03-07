using course.Server.Configs.Enums;
using course.Server.Data;

namespace course.Server.Models
{
    public class OrderUserInfoModel : OrderBaseInfoModel
    {
        public int UserId { get; set; }
        public string Address { get; set; }

        public OrderUserInfoModel() { }

        public OrderUserInfoModel(Order order, List<OrderRecord> records)
            : base(order, records)
        {
            UserId = order.UserId;
            Address = order.Address;
        }
    }
}
