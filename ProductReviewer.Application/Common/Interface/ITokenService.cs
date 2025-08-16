namespace ProductReviewer.Application.Common.Interface
{
    public interface ITokenService
    {
        string Generate(string userId, string userName);
    }
}
