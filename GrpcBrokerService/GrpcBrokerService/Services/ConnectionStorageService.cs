using Grpc.Broker.Interfaces;
using Grpc.Broker.Models;

namespace Grpc.Broker.Services;

public class ConnectionStorageService : IConnectionStorageService
{
    private readonly List<Connection> _connections;
    private readonly object _lock;

    public ConnectionStorageService()
    {
        _connections = new List<Connection>();
        _lock = new object();
    }

    public void Add(Connection connection)
    {
        lock (_lock)
        {
            _connections.Add(connection);
        }
    }

    public IList<Connection> GetConnectionsByTopic(string topic)
    {
        lock (_lock)
        {
            return _connections.Where(c => c.Topic == topic).ToList();
        }
    }

    public void Remove(string address)
    {
        lock (_lock)
        {
            _connections.RemoveAll(c => c.Address == address);
        }
    }
}
