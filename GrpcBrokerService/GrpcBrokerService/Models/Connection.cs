using Grpc.Net.Client;

namespace Grpc.Broker.Models;

public class Connection
{
    public string Address { get; }
    public string Topic { get; }
    public GrpcChannel Channel { get; }

    public Connection(string address, string topic)
    {
        Address = address;
        Topic = topic;
        Channel = GrpcChannel.ForAddress(address);
    }
}
