using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_impl.Server.Data
{
    [Table("stores")]
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public Seller Owner { get; set; }

        [Required]
        public int OwnerId { get; set; }
    }
}
