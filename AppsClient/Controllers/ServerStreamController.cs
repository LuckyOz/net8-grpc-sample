
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
    }
}
