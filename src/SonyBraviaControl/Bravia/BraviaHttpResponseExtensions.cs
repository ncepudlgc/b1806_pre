using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bravia;

public static class BraviaHttpResponseExtensions
{
    public static async Task<T> DeserializeAsync<T>(this BraviaHttpResponse response)
    {
        if (response.Result == null)
        {
            throw new Exception("no result found");
        }
        
        var json = JsonSerializer.Serialize(response.Result.FirstOrDefault());
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        using var stream = new MemoryStream(bytes);
        
        return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }).ConfigureAwait(false);
    }
}