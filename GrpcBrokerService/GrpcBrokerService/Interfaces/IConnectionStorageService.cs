using Grpc.Broker.Models;

namespace Grpc.Broker.Interfaces;

public interface IConnectionStorageService
{
    void Add(Connection connection);
    void Remove(string address);
    IList<Connection> GetConnectionsByTopic(string topic);
}
