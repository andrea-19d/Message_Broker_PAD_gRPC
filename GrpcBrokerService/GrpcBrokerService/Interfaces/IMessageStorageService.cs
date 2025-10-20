using Grpc.Broker.Models;

namespace Grpc.Broker.Interfaces;

public interface IMessageStorageService
{
    void Add(Message message);
    Message? GetNext();
    bool IsEmpty();

}
