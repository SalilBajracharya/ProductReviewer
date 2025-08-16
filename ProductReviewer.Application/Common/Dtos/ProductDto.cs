namespace ProductReviewer.Application.Common.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public string? ProductType { get; set; }
        public double AverageRating { get; set; }
        public string Category { get; set; } = null!;
    }
}
