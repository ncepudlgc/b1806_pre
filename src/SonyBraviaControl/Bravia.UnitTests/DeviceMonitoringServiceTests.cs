using Bravia.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bravia.UnitTests;

[TestFixture]
public class DeviceMonitoringServiceTests
{
    private static readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>(); 
    
    private static ISystemService _systemService;
    private static IDeviceMonitoringService _deviceMonitoringService;
    private static IAVContentService _avContentService;
    private static IConnectionManager _connectionManager;
    
    private readonly string _deviceId1 = Guid.NewGuid().ToString();
    private readonly string _deviceId2 = Guid.NewGuid().ToString();
    
    [OneTimeSetUp]
    public void Setup()
    {    
        var serviceProvider = new ServiceCollection()
            .AddBraviaServices()
            .BuildServiceProvider();
        
        _systemService = serviceProvider.GetRequiredService<ISystemService>();
        _deviceMonitoringService = serviceProvider.GetRequiredService<IDeviceMonitoringService>();
        _avContentService = serviceProvider.GetRequiredService<IAVContentService>();
        _connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();

        AddConnections();
    }
    
    [Test]
    public async Task IDeviceMonitoringService_ShouldBeReady()
    {
        await Task.Run(() =>
        {
            Assert.That(_deviceMonitoringService, Is.Not.Null);
        });
    }
    
    [Test]
    public async Task IDeviceMonitoringService_ShouldReceiveExpectedPowerStatusUpdate()
    {
        var expectedStatus = false;
        
        void ReceivePowerStatusUpdate(PowerStatus status)
        {
            if (string.IsNullOrWhiteSpace(status.Status))
            {
                return;
            }
            
            var expectedResult = expectedStatus ? "active" : "standby";
            
            Assert.That(status.Status?.Equals(expectedResult), Is.True);
        }       
        
        using IDisposable powerStatusSubscription1 = _deviceMonitoringService
            .PowerStatusUpdates(_deviceId1)
            .Subscribe(ReceivePowerStatusUpdate);
        
        using IDisposable powerStatusSubscription2 = _deviceMonitoringService
            .PowerStatusUpdates(_deviceId1)
            .Subscribe(ReceivePowerStatusUpdate);
        
        expectedStatus = true;
        
        var powerStatusDto = new SetPowerStatusDto()
        {
            Status = true
        };
        
        var setResponse = await _systemService.SetPowerStatusAsync(_deviceId1, powerStatusDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        
        expectedStatus = false;
        
        powerStatusDto = new SetPowerStatusDto()
        {
            Status = false
        };
        
        setResponse = await _systemService.SetPowerStatusAsync(_deviceId1, powerStatusDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
    }
    
    [Test]
    public async Task IDeviceMonitoringService_ShouldReceiveExpectedPlayContentUpdate()
    {
        var expectedContent = "Device1_TestContent1";

        void ReceivePlayContentUpdate(PlayContent content)
        {
            if (string.IsNullOrWhiteSpace(content.Uri))
            {
                return;
            }
            
            Assert.That(content.Uri?.Equals(expectedContent), Is.True, $"Unexpected result: {content.Uri}");
        }      
        
        using IDisposable playContentSubscription1 = _deviceMonitoringService
            .PlayContentUpdates(_deviceId1)
            .Subscribe(ReceivePlayContentUpdate);
        
        using IDisposable playContentSubscription2 = _deviceMonitoringService
            .PlayContentUpdates(_deviceId1)
            .Subscribe(onNext: ReceivePlayContentUpdate);

        var playContentDto = new SetPlayContentDto()
        {
            Uri = expectedContent
        };
        
        var setResponse = await _avContentService.SetPlayContentAsync(_deviceId1, playContentDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
        
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        
        expectedContent = "TestContent2";
        
        playContentDto = new SetPlayContentDto()
        {
            Uri = expectedContent
        };
        
        setResponse = await _avContentService.SetPlayContentAsync(_deviceId1, playContentDto).ConfigureAwait(false);
        
        Assert.That(setResponse, Is.True);
    }
    
    [Test]
    public async Task IDeviceMonitoringService_ShouldCompleteObservablesOnDeviceDisconnect()
    {
        void ReceivePlayContentUpdate(PlayContent content)
        {
        }     
        void ReceivePowerStatusUpdate(PowerStatus content)
        {
        }       
        
        bool device1Disconnected = false;
        bool device2Disconnected = false;
        
        using IDisposable device1Subscription = _deviceMonitoringService
            .PlayContentUpdates(_deviceId1)
            .Subscribe(
                onNext: ReceivePlayContentUpdate,
                onCompleted: () =>
                {
                    device1Disconnected = true;
                });
               
        using IDisposable device2Subscription = _deviceMonitoringService
            .PowerStatusUpdates(_deviceId2)
            .Subscribe(
                onNext: ReceivePowerStatusUpdate,
                onCompleted: () =>
                {
                    device2Disconnected = true;
                });

        await Task.Delay(500);
        
        _connectionManager?.RemoveConnection(_deviceId1);
        _connectionManager?.RemoveConnection(_deviceId2);
        
        await Task.Delay(1000);
        
        Assert.That(device1Disconnected, Is.True, $"{_deviceId1} was not removed.");
        Assert.That(device2Disconnected, Is.True, $"{_deviceId2} was not removed.");
        
        // Add the connections to ensure the next test does not fail.
        AddConnections();
        
        Assert.That(_connectionManager.GetConnectionIds().Contains(_deviceId1), Is.True, $"{_deviceId1} was not added.");
        Assert.That(_connectionManager.GetConnectionIds().Contains(_deviceId2), Is.True, $"{_deviceId2} was not added.");
    }

    private void AddConnections()
    {
        _connectionManager?.AddConnection(_deviceId1, new()
        {
            Id = _deviceId1,
            Hostname = "localhost:5000"
        });
        _connectionManager?.AddConnection(_deviceId2, new()
        {
            Id = _deviceId2,
            Hostname = "localhost:5000"
        });
    }
}