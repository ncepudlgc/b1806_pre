using System.Collections.Concurrent;
using System.Text.Json;
using Bravia.Abstractions;

namespace Bravia.DeviceHost;

public class ConnectionService
{
    private readonly ConcurrentDictionary<string, Connection> Connections = new();
    
    public Task HandleRemoveConnectionAsync(string id)
    {
        if (!Connections.TryRemove(id, out var connection))
        {
            throw new Exception($"connection id '{id}' not found");
        }
        
        return Task.FromResult(JsonSerializer.Serialize(true));
    }
    
    private Task<string> HandleGetConnectionsRequestAsync()
    {
        var result = JsonSerializer.Serialize(Connections.Values);
        
        return Task.FromResult(result);
    }
    
    private Task<string> HandleGetConnectionRequestAsync(string id)
    {
        if (!Connections.ContainsKey(id))
        {
            throw new Exception($"connection id: {id} not found.");
        }
        
        return Task.FromResult(JsonSerializer.Serialize(Connections[id]));
    }
    
    private Task<string> HandleAddConnectionRequestAsync(string id, Connection connection)
    {
        if (!Connections.TryGetValue(id, out var powerState))
        {
            throw new Exception($"connection id '{id}' not found");
        }

        Connections[id] = connection;
        
        return Task.FromResult(JsonSerializer.Serialize(true));
    }
}