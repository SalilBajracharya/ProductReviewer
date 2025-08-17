namespace ProductReviewer.Application.Common.Interface
{
    public interface ICsvGenerator
    {
        string Export<T>(IEnumerable<T> data);
    }
}
