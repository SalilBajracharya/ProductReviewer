using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductReviewer.Domain.Entities;

namespace ProductReviewer.Infrastructure.Data.Configurations
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(200);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
