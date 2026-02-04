using Bravia.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bravia.UnitTests;

[NonParallelizable]
[TestFixture]
public class AVContentServiceTests
{
    private static IAVContentService? _avContentService;
    
    private readonly string _deviceId = Guid.NewGuid().ToString();

    [OneTimeSetUp]
    public void Setup()
    {    
        var serviceProvider = new ServiceCollection()
            .AddBraviaServices()
            .BuildServiceProvider();
        
        _avContentService = serviceProvider.GetRequiredService<IAVContentService>();
        var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();

        // Used by system service
        connectionManager?.AddConnection(_deviceId, new()
        {
            Id = _deviceId,
            Hostname = "localhost:5000"
        });
    }
    
    [Test]
    public async Task AVContentService_ShouldBeReady()
    {
        await Task.Run(() =>
        {
            Assert.That(_avContentService, Is.Not.Null);
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
    public async Task AVContentService_SetPlayContent_ShouldUpdateDeviceHost()
    {
        var setPlayContentDto = new SetPlayContentDto()
        {
            Uri = "Test"
        };
        
        var setResponse = await _avContentService.SetPlayContentAsync(_deviceId, setPlayContentDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        PlayContent getResponse = await _avContentService.GetPlayingContentInfoAsync(_deviceId).ConfigureAwait(false);
        
        Assert.That(getResponse, Is.Not.Null);
        Assert.That(getResponse.Uri.Equals("Test"), Is.True);
        
        setPlayContentDto = new SetPlayContentDto()
        {
            Uri = "Temp"
        };
        
        setResponse = await _avContentService.SetPlayContentAsync(_deviceId, setPlayContentDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        getResponse = await _avContentService.GetPlayingContentInfoAsync(_deviceId).ConfigureAwait(false);
        
        Assert.That(getResponse, Is.Not.Null);
        Assert.That(getResponse.Uri.Equals("Test"), Is.False);
        Assert.That(getResponse.Uri.Equals("Temp"), Is.True);
    }
}
