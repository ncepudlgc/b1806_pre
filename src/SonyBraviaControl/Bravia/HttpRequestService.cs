using System.Text;
using System.Text.Json;
using Bravia.Abstractions;

namespace Bravia;

internal class HttpRequestService : IHttpRequestService
{
    private const string AuthHeader = "X-Auth-PSK";
    
    private static readonly HttpClient HttpClient = new();
    
    public HttpRequestService()
    {
    }
    
    public async Task<BraviaHttpResponse> SendAsync(
        string path,
        Connection connection,
        BraviaHttpRequest request)
    {
        string body = await request.SerializeAsync().ConfigureAwait(false);
        var uri = new Uri($"http://{connection.Hostname}/sony/{path}");
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        
        requestMessage.Headers.Add(AuthHeader, connection.PublicKey);
        
        var response = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false);
        
        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        
        return await responseStream.DeserializeAsync().ConfigureAwait(false);
    }
    
    public void Dispose()
    {
        HttpClient.Dispose();
    }
}