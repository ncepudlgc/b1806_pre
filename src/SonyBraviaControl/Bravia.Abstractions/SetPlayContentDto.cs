using System.Text.Json.Serialization;

namespace Bravia.Abstractions;

public class SetPlayContentDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; init; }
}