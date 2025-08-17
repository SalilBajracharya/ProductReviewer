namespace ProductReviewer.Application.Common.Dtos
{
    public class ReviewProductDto
    {
        public int ProductInt { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
    }
}
