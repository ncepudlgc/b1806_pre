using Bravia.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bravia.UnitTests;

[NonParallelizable]
[TestFixture]
public class SystemServiceTests
{
    private static ISystemService? _systemService;
    
    private readonly string _deviceId = Guid.NewGuid().ToString();

    [OneTimeSetUp]
    public void Setup()
    {    
        var serviceProvider = new ServiceCollection()
            .AddBraviaServices()
            .BuildServiceProvider();
        
        _systemService = serviceProvider.GetRequiredService<ISystemService>();
        var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();

        // Used by system service
        connectionManager?.AddConnection(_deviceId, new()
        {
            Id = _deviceId,
            Hostname = "localhost:5000"
        });
    }
    
    [Test]
    public async Task SystemService_ShouldBeReady()
    {
        await Task.Run(() =>
        {
            Assert.That(_systemService, Is.Not.Null);
        });
    }
    
    [Test]
    public async Task DeviceHost_ShouldBeRunning()
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        try
        {
            var response = await httpClient!.GetAsync("http://localhost:5000");
            Assert.That(response, Is.Not.Null, "DeviceHost should be responding on localhost:5000");
            Console.WriteLine($"DeviceHost responded with status: {response.StatusCode}.");
        }
        catch (HttpRequestException ex)
        {
            Assert.Fail($"DeviceHost is not accessible on localhost:5000: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Assert.Fail("DeviceHost request timed out - service may not be running");
        }
    }
    
    [Test]
    public async Task SystemService_SetPowerStatus_ShouldUpdateDeviceHost()
    {
        var powerStatusDto = new SetPowerStatusDto()
        {
            Status = true
        };
        
        var setResponse = await _systemService.SetPowerStatusAsync(_deviceId, powerStatusDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        PowerStatus getResponse = await _systemService.GetPowerStatusAsync(_deviceId).ConfigureAwait(false);
        
        Assert.That(getResponse, Is.Not.Null);
        Assert.That(getResponse.Status.Equals("active"), Is.True);
        
        powerStatusDto = new SetPowerStatusDto()
        {
            Status = false
        };
        
        setResponse = await _systemService.SetPowerStatusAsync(_deviceId, powerStatusDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        getResponse = await _systemService.GetPowerStatusAsync(_deviceId).ConfigureAwait(false);
        
        Assert.That(getResponse, Is.Not.Null);
        Assert.That(getResponse.Status.Equals("standby"), Is.True);
    }
}