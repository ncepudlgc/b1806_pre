using Bravia.Abstractions;

namespace Bravia;

public interface ISystemRequestFactory
{
    BraviaHttpRequest GetPowerStatusRequest();
    BraviaHttpRequest SetPowerStatusRequest(SetPowerStatusDto status);
}