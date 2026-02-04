using System.Collections.Concurrent;
using System.Text.Json;
using Bravia.Abstractions;

namespace Bravia.DeviceHost;

public class AVContentService : BraviaService
{
    private const string SetPlayContentMethod = "setPlayContent";
    private const string GetPlayContentMethod = "getPlayingContentInfo";
    
    private readonly ConcurrentDictionary<string, string> ContentStates = new();
    
    public override Task<string> ProcessRequestAsync(string hostname, BraviaHttpRequest request)
    {
        if (request.Method.Equals(GetPlayContentMethod))
        {
            return HandleGetPlayingContentRequestAsync(hostname, request);
        }
        if (request.Method.Equals(SetPlayContentMethod))
        {
            return HandleSetPlayContentRequestAsync(hostname, request);
        }
        
        throw new Exception("Unknown method");
    }

    public void RemoveConnectionId(string connectionId)
    {
        ContentStates.TryRemove(connectionId, out _);
    }

    private Task<string> HandleGetPlayingContentRequestAsync(string hostname, BraviaHttpRequest request)
    {
        if (ContentStates.TryGetValue(hostname, out var contentStatus) == false)
        {
            throw new Exception($"connection '{hostname}' not found");
        }

        if (!request.Id.Equals(103))
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
            Id = 103,
            Result =
            [
                new Dictionary<string, string>()
                {
                    { "uri", ContentStates[hostname] },
                    { "source", "" },
                    { "title", "" }
                }
            ]
        });
        
        return Task.FromResult(response);
    }

    private Task<string> HandleSetPlayContentRequestAsync(string hostname, BraviaHttpRequest request)
    {
        if (request.Params[0]?.TryGetValue("uri", out var statusObj) == true)
        {
            if (request.Params?.Count != 1 || (statusObj is not JsonElement element))
            {
                return Task.FromResult("Invalid uri.");
            }
    
            if (!request.Id.Equals(101))
            {
                return Task.FromResult("Invalid id.");
            }
        
            if (!request.Version.Equals("1.0"))
            {
                return Task.FromResult("Invalid version.");
            }
            
            // This directly addresses the NullReferenceException thrown in the renamed 'AVContentService_SetPlayContent_ShouldUpdateDeviceHost' test.
            // The original method was serializing the request object to a string, then deserializing it into the PLayContent object, which meant the Uri was null.
            // By ensuring this is a 'JsonElement' type, we can parse the expected value and store it. 
            string? status = element.GetString();

            if (string.IsNullOrWhiteSpace(status))
            {
                return Task.FromResult("Status cannot be null.");
            }
            
            ContentStates[hostname] = status;

            var response = JsonSerializer.Serialize(new BraviaHttpResponse
            {
                Id = 101,
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