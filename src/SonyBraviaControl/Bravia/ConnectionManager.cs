using System.Collections.Concurrent;
using System.Data;
using Bravia.Abstractions;

namespace Bravia;

internal class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<string, Connection> Connections;
    
    public ConnectionManager()
    {
        Connections = new ConcurrentDictionary<string, Connection>();
    }
    
    public bool RemoveConnection(string deviceId)
    {
        Connections.TryRemove(deviceId, out var connection);
        
        return connection != null;
    }

    public bool AddConnection(string deviceId, Connection connection)
    {
        if (Connections.ContainsKey(deviceId))
        {
            return false;
        }
        
        return Connections.TryAdd(deviceId, connection);
    }

    public Connection GetConnection(string deviceId)
    {
        if (!Connections.TryGetValue(deviceId, out var connection))
        {
            throw new DataException("Connection not found");
        }

        return connection;
    }

    public IEnumerable<Connection> GetConnections()
    {
        return Connections.Values;
    }
    
    public IEnumerable<string> GetConnectionIds()
    {
        return Connections.Keys;
    }
}