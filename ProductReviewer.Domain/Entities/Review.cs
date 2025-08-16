namespace ProductReviewer.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string UserId { get; set; } = null!;

        public double Rating { get; set; }
        public string? Comment { get; set; }
    }
}
