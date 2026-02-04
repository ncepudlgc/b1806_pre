namespace Bravia.DeviceHost;

public abstract class BraviaService
{
    public abstract Task<string> ProcessRequestAsync(string id, BraviaHttpRequest request);
}