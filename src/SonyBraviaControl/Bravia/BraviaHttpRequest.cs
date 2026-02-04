using System.Text.Json.Serialization;

namespace Bravia;

public record BraviaHttpRequest
{
    [JsonPropertyName("method")]
    public string Method { get; init; }
    [JsonPropertyName("id")]
    public int Id { get; init; }
    [JsonPropertyName("version")]
    public string Version { get; init; }
    [JsonPropertyName("params")]
    public List<Dictionary<string,object>> Params { get; init; }
}