using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bravia;

public static class StreamExtensions
{
    public static async Task<BraviaHttpResponse> DeserializeAsync(this Stream response)
    {
        return await JsonSerializer.DeserializeAsync<BraviaHttpResponse>(response).ConfigureAwait(false);
    }
}