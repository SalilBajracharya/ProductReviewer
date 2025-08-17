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
    }
}
