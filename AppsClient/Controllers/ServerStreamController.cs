
using AppsClient.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppsClient.Controllers
{
    public class ServerStreamController : ControllerBase
    {
        [HttpGet("get_all_product_server_stream")]
        public async Task<IActionResult> GetAllProductServerStream()
        {
            var grpcChannel = GrpcChannel.ForAddress("http://localhost:5292");
            var client = new Product.ProductClient(grpcChannel);
            using var streamingCall = client.GetServerStreamAllProduct(new Empty());

            await foreach (var data in streamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{JsonConvert.SerializeObject(data)}");
            }

            return Ok("Success");
        }

        [HttpGet("get_all_product_server_stream_timeout")]
        public async Task<IActionResult> GetAllProductServerStreamTimeOut()
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var grpcChannel = GrpcChannel.ForAddress("http://localhost:5292");
                var client = new Product.ProductClient(grpcChannel);
                using var streamingCall = client.GetServerStreamAllProduct(new Empty(), cancellationToken: cts.Token);

                await foreach (var data in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    Console.WriteLine($"{JsonConvert.SerializeObject(data)}");
                }
            } catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            

            return Ok("Success");
        }
    }
}
