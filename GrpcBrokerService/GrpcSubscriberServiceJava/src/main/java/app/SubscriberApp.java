package app;

import java.io.BufferedReader;
import java.io.InputStreamReader;

import GrpcAgent.SubscribeReply;
import GrpcAgent.SubscribeRequest;
import io.grpc.Server;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.netty.shaded.io.grpc.netty.NettyServerBuilder;

// Service stubs live here:
import GrpcAgent.SubscriberGrpc;
// Messages are nested in the outer class generated from subscribe.proto:

public class SubscriberApp {
    public static void main(String[] args) throws Exception {
        // 1) start local Notifier server on free port (0)
        Server server = NettyServerBuilder.forPort(0)
                .addService(new NotifierService())
                .build()
                .start();

        int actualPort = server.getPort();
        String subscriberAddress = "http://localhost:" + actualPort; // h2c callback
        System.out.println("Notifier listening on " + subscriberAddress);

        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
            try { server.shutdownNow(); } catch (Exception ignored) {}
        }));

        // 2) read topic
        System.out.print("Topic: ");
        BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
        String topic = br.readLine();
        if (topic == null || topic.trim().isEmpty()) {
            System.err.println("Error: Topic cannot be empty!");
            server.shutdownNow();
            return;
        }
        topic = topic.trim().toLowerCase();

        // 3) call Broker's Subscribe (PLAINTEXT h2c)
        String brokerHost = "127.0.0.1";
        int brokerPort = 50051;

        ManagedChannel brokerChannel = ManagedChannelBuilder
                .forAddress(brokerHost, brokerPort)
                .usePlaintext()
                .build();

        SubscriberGrpc.SubscriberBlockingStub stub = SubscriberGrpc.newBlockingStub(brokerChannel);

        // Use the nested classes from SubscribeOuterClass
        SubscribeRequest req = SubscribeRequest.newBuilder()
                .setTopic(topic)
                .setAddress(subscriberAddress)
                .build();

        try {
            SubscribeReply reply = stub.subscribe(req);
            System.out.println("Subscribe sent. isSuccess=" + reply.getIsSuccess());
        } catch (Exception e) {
            System.err.println("Error subscribing: " + e.getMessage());
        }

        System.out.println("Waiting for notifications... Press ENTER to exit.");
        br.readLine();

        brokerChannel.shutdownNow();
        server.shutdownNow();
    }
}
