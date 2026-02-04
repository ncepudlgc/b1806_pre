namespace Bravia.Abstractions;

public record PlayContent : Status
{
    /// <summary>
    ///     The URI to the current playing source; extInput:hdmi?port=1.
    /// </summary>
    public string Uri { get; init; }
    public string Title { get; init; }
    public string Source { get; init; }
}