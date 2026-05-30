using marketplace_impl.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models
{
    public class StorePostModel
    {
        [Required]
        public required string Name { get; set; }

        public Store ToEntity(int ownerId)
        {
            return new Store
            {
                Name = Name,
                OwnerId = ownerId
            };
        }
    }
}
