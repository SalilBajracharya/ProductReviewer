namespace ProductReviewer.Application.Common.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public string? ProductType { get; set; }
    }
}
