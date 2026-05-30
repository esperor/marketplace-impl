using marketplace_impl.Server.Data;

namespace marketplace_impl.Server.Models
{
    public class StoreInfoModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public StoreInfoModel(Store store)
        {
            Id = store.Id;
            Name = store.Name;
        }
    }
}
