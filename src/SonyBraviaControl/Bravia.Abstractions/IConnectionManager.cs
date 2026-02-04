namespace Bravia.Abstractions;

public interface IConnectionManager
{
    bool RemoveConnection(string deviceId);
    bool AddConnection(string deviceId, Connection connection);
    Connection GetConnection(string deviceId);
    IEnumerable<Connection> GetConnections();
    IEnumerable<string> GetConnectionIds();
}