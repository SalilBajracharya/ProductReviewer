using Microsoft.AspNetCore.Identity;

namespace ProductReviewer.Infrastructure.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}
