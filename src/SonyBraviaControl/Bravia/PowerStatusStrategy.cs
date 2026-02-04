using Bravia.Abstractions;

namespace Bravia;

internal class PowerStatusStrategy : IDeviceStatusStrategy<PowerStatus>
{
    private readonly ISystemService _systemService;
    private readonly IObservable<PowerStatus> _statusObservable;

    public PowerStatusStrategy(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public async Task<PowerStatus> CheckStatusAsync(string deviceId)
    {
        return await _systemService.GetPowerStatusAsync(deviceId);
    }
}