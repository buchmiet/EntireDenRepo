
namespace AmazonSPAPIClient;

public interface IHasNextToken
{
    string GetNextToken();
    void Merge(IHasNextToken anotherResponse);
}