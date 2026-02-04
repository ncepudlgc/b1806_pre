using System.Text.Json.Serialization;

namespace Bravia.Abstractions;

public record Connection
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("hostname")]
    public string Hostname { get; init; }
    [JsonPropertyName("publicKey")]
    public string PublicKey { get; init; }
}