namespace ProductReviewer.Application.Common.Interface
{
    public interface IAuthService
    {
        Task<string> LoginValidate(string username, string password);
    }
}
