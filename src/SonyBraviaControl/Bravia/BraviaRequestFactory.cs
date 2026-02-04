namespace Bravia;

internal class BraviaRequestFactory : IBraviaRequestFactory
{
    private readonly ISystemRequestFactory _systemRequestFactory;
    private readonly IAVContentRequestFactory _avContentRequestFactory;
    
    public ISystemRequestFactory SystemRequestFactory => _systemRequestFactory;
    public IAVContentRequestFactory AVContentRequestFactory => _avContentRequestFactory;

    public BraviaRequestFactory(
        ISystemRequestFactory systemRequestFactory,
        IAVContentRequestFactory avContentRequestFactory)
    {
        _systemRequestFactory = systemRequestFactory;
        _avContentRequestFactory = avContentRequestFactory;
    }
}