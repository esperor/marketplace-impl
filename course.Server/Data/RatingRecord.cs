using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace course.Server.Data
{
    [Table("rating_record")]
    public class RatingRecord
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(OrderRecordId))]
        public OrderRecord OrderRecord { get; set; }

        [Required]
        public int OrderRecordId { get; set; }

        public DateOnly Date { get; set; }

        public string? Comment { get; set; }

        public int RatingValue { get; set; }
    }
}
