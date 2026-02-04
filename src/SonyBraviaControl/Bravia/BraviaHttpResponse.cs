using System.Text.Json.Serialization;

namespace Bravia;

public record BraviaHttpResponse
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    [JsonPropertyName("result")]
    public Dictionary<string,string>[] Result { get; init; }
}