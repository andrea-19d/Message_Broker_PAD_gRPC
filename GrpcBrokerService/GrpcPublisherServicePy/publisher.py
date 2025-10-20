import os
import grpc
from grpc_agent import publish_pb2 as publisher_pb2
from grpc_agent import publish_pb2_grpc as publisher_pb2_grpc

def get_broker_target() -> str:
    return os.getenv("BROKER_ADDRESS", "127.0.0.1:50051")

def main():
    target = get_broker_target()

    # PLAINTEXT h2c
    channel = grpc.insecure_channel(target)
    client = publisher_pb2_grpc.PublisherStub(channel)

    print(f"Python publisher connected to {target}. Ctrl+C to exit.\n")

    try:
        while True:
            topic = input("Topic: ").strip().lower()
            content = input("Content: ").strip()

            req = publisher_pb2.PublishRequest(topic=topic, content=content)
            try:
                reply = client.PublishMessage(req, timeout=5)
                print("Published OK.\n" if reply.isSuccess else "Publish reported failure.\n")
            except Exception as ex:
                print(f"Error publishing the message: {ex}\n")
    except KeyboardInterrupt:
        print("\nBye.")

if __name__ == "__main__":
    main()
