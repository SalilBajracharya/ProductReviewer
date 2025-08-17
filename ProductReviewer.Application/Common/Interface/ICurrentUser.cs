using Microsoft.Extensions.Primitives;

namespace ProductReviewer.Application.Common.Interface
{
    public interface ICurrentUser
    {
        string? UserId { get; }
    }
}
