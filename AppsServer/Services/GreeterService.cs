using Grpc.Core;
using AppsServer.Protos;

namespace AppsServer.Services;

public class GreeterService() : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
