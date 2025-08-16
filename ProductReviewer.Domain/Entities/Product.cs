using ProductReviewer.Domain.Enums;

namespace ProductReviewer.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ProductType { get; set; }
        public string? SKU { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0.0;
        public ProductCategory Category =>
            AverageRating >= 4.0 ? ProductCategory.Good :
            AverageRating >= 2.0 ? ProductCategory.Bad :
            ProductCategory.Worst;
    }
}
