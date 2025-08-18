namespace ProductReviewer.Application.Common.Dtos
{
    public class ReviewProductDto
    {
        public int ProductId { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
    }
}
