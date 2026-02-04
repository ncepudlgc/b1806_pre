namespace Bravia;

public interface IBraviaRequestFactory
{
    ISystemRequestFactory SystemRequestFactory { get; }
    IAVContentRequestFactory AVContentRequestFactory { get; }
}