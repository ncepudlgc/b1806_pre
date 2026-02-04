using Bravia.Abstractions;

namespace Bravia;

internal interface IHttpRequestService : IDisposable
{
    Task<BraviaHttpResponse> SendAsync(
        string path,
        Connection connection,
        BraviaHttpRequest request);
}