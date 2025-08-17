namespace ProductReviewer.Application.Common.Interface
{
    public interface ITokenService
    {
        Task<string> Generate(string userId, string userName);
    }
}
