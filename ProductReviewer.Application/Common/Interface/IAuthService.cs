using FluentResults;
using ProductReviewer.Application.Common.Dtos;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IAuthService
    {
        Task<Result<string>> LoginValidate(string username, string password);
        Task<Result<string>> CreateUser(UserDto user);
        Task<Result<string>> AssignRole(string userId, string roleName);
    }
}
