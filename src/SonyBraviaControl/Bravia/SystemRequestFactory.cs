using Bravia.Abstractions;

namespace Bravia;

internal class SystemRequestFactory : ISystemRequestFactory
{
    public BraviaHttpRequest GetPowerStatusRequest()
    {        
        return new BraviaHttpRequest
        {
            Id = 50,
            Version="1.0",
            Method="getPowerStatus",
            Params = []
        };
    }

    public BraviaHttpRequest SetPowerStatusRequest(SetPowerStatusDto status)
    {
        ArgumentNullException.ThrowIfNull(status, nameof(status));
        
        return new BraviaHttpRequest
        {
            Id = 55,
            Version = "1.0",
            Method = "setPowerStatus",
            Params = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "status", status.Status } }
            }
        };
    }
}