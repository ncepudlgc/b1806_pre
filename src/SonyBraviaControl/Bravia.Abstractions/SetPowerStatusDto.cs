using System.Text.Json.Serialization;

namespace Bravia.Abstractions;

public record SetPowerStatusDto()
{
    [JsonPropertyName("status")]
    public bool Status { get; init; }
}