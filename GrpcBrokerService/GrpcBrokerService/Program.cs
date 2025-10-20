using Grpc.Broker.Services;
using Grpc.Broker.Interfaces;
using GrpcBrokerService.Services;
using GrpcDS.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Do NOT call UseUrls(...) here

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(EndpointConsts.BrokerPort, lo =>
    {
        lo.Protocols = HttpProtocols.Http2; // <-- h2c (HTTP/2 without TLS)
        // DO NOT call lo.UseHttps()
    });
});

builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageStorageService, MessageStorageService>();
builder.Services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
builder.Services.AddHostedService<SenderWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// No HTTPS redirection in h2c mode
app.MapGet("/", () => "Broker is up");
app.MapGrpcService<PublisherService>();
app.MapGrpcService<SubscriberService>();

app.Run();