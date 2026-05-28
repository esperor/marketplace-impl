namespace course.Server.Models
{
    public class ProductReviewModel
    {
        public required DateOnly RatingDate { get; set; }
        public required int RatingValue { get; set; }
        public string? RatingComment { get; set; }
    }
}
