using ProductReviewer.Application.Common.Dtos;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IAuthService
    {
        Task<string> LoginValidate(string username, string password);
        Task<string> CreateUser(UserDto user);
        Task<string> AssignRole(string userId, string roleName);
    }
}
