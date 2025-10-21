package app;

import GrpcAgent.NotifierGrpc;
import GrpcAgent.Notify;
import io.grpc.stub.StreamObserver;

public class NotifierService extends NotifierGrpc.NotifierImplBase {
    @Override
    public void notify(Notify.NotifyRequest request, StreamObserver<Notify.NotifyReply> responseObserver) {
        System.out.println(request.getContent()); // print the message
        Notify.NotifyReply reply = Notify.NotifyReply.newBuilder().setIsSuccess(true).build();
        responseObserver.onNext(reply);
        responseObserver.onCompleted();
    }
}
