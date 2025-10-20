using Grpc.Broker.Interfaces;
using Grpc.Broker.Models;
using GrpcAgent;

namespace Grpc.Broker.Services;

public class SubscriberService : Subscriber.SubscriberBase
{
    private readonly IConnectionStorageService _connectionStorage;
    public SubscriberService(IConnectionStorageService connectionStorage)
    {
        _connectionStorage = connectionStorage;
    }

    public override Task<SubscribeReply> Subscribe(SubscribeRequest request, Grpc.Core.ServerCallContext context)
    {
        try
        {
            var connection = new Connection(request.Address, request.Topic);
            _connectionStorage.Add(connection);
            Console.WriteLine($"New subscription: {request.Address} - {request.Topic}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connection: {ex.Message}");
        }

        return Task.FromResult(new SubscribeReply { IsSuccess = true });
    }
}
