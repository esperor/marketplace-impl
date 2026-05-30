using marketplace_impl.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace marketplace_impl.Server.Models
{
    public class ProductPostModel
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public ProductPostModel() { }

        public Product ToEntity()
        {
            var entity = new Product
            {
                StoreId = StoreId,
                Title = Title,
                Description = Description
            };
            return entity;
        }
    }
}
