using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Bravia.UnitTests;

[SetUpFixture]
public class DeviceHostFixture
{
    private IHost? _host;
    private Task? _hostTask;
    private CancellationTokenSource? _cancellationTokenSource;

    [OneTimeSetUp]
    public async Task StartDeviceHost()
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Bravia.DeviceHost.Startup>();
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenLocalhost(5000);
                    });
                });

            _host = hostBuilder.Build();
            
            _hostTask = _host.RunAsync(_cancellationTokenSource.Token);
            
            await Task.Delay(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start Bravia.DeviceHost: {ex.Message}");
            throw;
        }
    }

    [OneTimeTearDown]
    public async Task StopDeviceHost()
    {
        try
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                
                if (_hostTask != null)
                {
                    await _hostTask.WaitAsync(TimeSpan.FromSeconds(10));
                }
            }
            
            _host?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping Bravia.DeviceHost: {ex.Message}");
        }
    }
}
