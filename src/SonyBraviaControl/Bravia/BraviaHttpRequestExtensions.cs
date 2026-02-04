using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bravia;

public static class BraviaHttpRequestExtensions
{
    public static async Task<string> SerializeAsync(this BraviaHttpRequest request)
    {
        using var stream = new MemoryStream();
        
        await JsonSerializer.SerializeAsync(stream, request);

        stream.Position = 0;

        using var reader = new StreamReader(stream);
        
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
}