
using Grpc.Broker.Interfaces;
using GrpcAgent;

namespace Grpc.Broker.Services;

public class SenderWorker : IHostedService
{
    private readonly IMessageStorageService _messageStorage;
    private readonly IConnectionStorageService _connectionStorage;
    private Timer _timer;
    private const int TimeToWait = 2000;

    public SenderWorker(IServiceScopeFactory serviceScopeFactory)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            _messageStorage = scope.ServiceProvider.GetRequiredService<IMessageStorageService>();
            _connectionStorage = scope.ServiceProvider.GetRequiredService<IConnectionStorageService>();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoSendWork, null, 0, TimeToWait);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void DoSendWork(object? state)
    {
        while(!_messageStorage.IsEmpty())
        {
            var message = _messageStorage.GetNext();
            if(message is not null)
            {
                var connections = _connectionStorage.GetConnectionsByTopic(message.Topic);

                foreach(var connection in connections)
                {
                    var client = new Notifier.NotifierClient(connection.Channel);
                    var request = new NotifyRequest { Content = message.Content };

                    try
                    {
                        var reply = client.Notify(request);
                        Console.WriteLine($"Notified subscriber {connection.Address} : {message.Content}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error notifying subscriber {connection.Address} : {ex.Message}");
                    }

                }
            }
        }
    }
}
