using System.Collections.Concurrent;
using System.Text.Json;
using Bravia.Abstractions;

namespace Bravia.DeviceHost;

public class SystemService : BraviaService
{
    private const string GetPowerStatusMethod = "getPowerStatus";
    private const string SetPowerStatusMethod = "setPowerStatus";
    
    private readonly ConcurrentDictionary<string, bool> PowerStates = new();
    
    public override Task<string> ProcessRequestAsync(string hostname, BraviaHttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(hostname);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Method);
        
        if (string.IsNullOrWhiteSpace(hostname))
        {
            return Task.FromResult($"Connection '{hostname}' not found");
        }

        if (string.IsNullOrWhiteSpace(request.Method))
        {
            return Task.FromResult($"Missing method.");
        }
        
        if (request.Method.Equals(GetPowerStatusMethod))
        {
            return HandleGetPowerStatusRequestAsync(hostname, request);
        }
        if (request.Method.Equals(SetPowerStatusMethod))
        {
            return HandleSetPowerStatusRequestAsync(hostname, request);
        }

        return Task.FromResult($"Unknown method '{request.Method}'");
    }
    
    private Task<string> HandleGetPowerStatusRequestAsync(string hostname, BraviaHttpRequest request)
    {
        if (PowerStates.TryGetValue(hostname, out var powerState) == false)
        {
            throw new Exception($"connection '{hostname}' not found");
        }

        if (!request.Id.Equals(50))
        {
            return Task.FromResult("Invalid id.");
        }
        
        if (!request.Version.Equals("1.0"))
        {
            return Task.FromResult("Invalid version.");
        }
        
        if (request.Params?.Count > 0)
        {
            return Task.FromResult("Invalid parameters.");
        }
        
        var response = JsonSerializer.Serialize(new BraviaHttpResponse
        { 
            Id = 50,
            Result =
            [
                new Dictionary<string, string>()
                {
                    { "status", PowerStates[hostname] ? "active" : "standby" }
                }
            ]
        });
        
        return Task.FromResult(response);
    }

    private Task<string> HandleSetPowerStatusRequestAsync(string hostname, BraviaHttpRequest request)
    {
        if (request.Params[0]?.TryGetValue("status", out var statusObj) == true)
        {
            if (request.Params?.Count != 1 || (statusObj is not JsonElement element))
            {
                return Task.FromResult("Invalid status");
            }
    
            if (!request.Id.Equals(55))
            {
                return Task.FromResult("Invalid id.");
            }
        
            if (!request.Version.Equals("1.0"))
            {
                return Task.FromResult("Invalid version.");
            }
            
            bool status = element.GetBoolean();
        
            PowerStates[hostname] = status;

            var response = JsonSerializer.Serialize(new BraviaHttpResponse
            {
                Id = 55,
                Result = []
            });
        
            return Task.FromResult(response);
        }
        else
        {
            return Task.FromResult("Invalid status");
        }
    }
}