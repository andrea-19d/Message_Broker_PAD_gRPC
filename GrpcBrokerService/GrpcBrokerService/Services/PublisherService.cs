using Grpc.Broker.Interfaces;
using Grpc.Broker.Models;
using Grpc.Core;
using GrpcAgent;

namespace GrpcBrokerService.Services;

public class PublisherService : Publisher.PublisherBase
{
    private readonly IMessageStorageService _messageStorage; 

    public PublisherService(IMessageStorageService messageStorage)
    {
        _messageStorage = messageStorage;
    }

    public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Received: {request.Topic} - {request.Content}");

        var message = new Message(request.Topic, request.Content);
        _messageStorage.Add(message);

        return Task.FromResult(new PublishReply { IsSuccess = true});
    }
}
