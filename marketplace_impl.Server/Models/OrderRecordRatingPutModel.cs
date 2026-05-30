namespace marketplace_impl.Server.Models
{
    public class OrderRecordRatingPutModel
    {
        public required int Id { get; set; }
        public required int RatingValue { get; set; }
        public string? RatingComment { get; set; }
    }
}
