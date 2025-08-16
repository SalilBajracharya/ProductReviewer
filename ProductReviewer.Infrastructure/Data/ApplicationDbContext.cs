using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {       
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("PrUsers");
            builder.Entity<IdentityRole>().ToTable("PrRoles");
            builder.Entity<IdentityUserRole<string>>().ToTable("PrUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("PrUserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("PrUserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("PrRoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("PrUserToken");
        }
    }
}
