namespace Bravia.Abstractions;

public record PowerStatus : Status
{
    /// <summary>
    ///     The current standby state; active or standby.
    /// </summary>
    public string Status { get; init; }
}