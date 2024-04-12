
using AppsClient.Protos;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AppsClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientStreamController : ControllerBase
    {
        [HttpPost("create_product_client_stream")]
        public async Task<IActionResult> CreateProductClientStream(List<ProductAddRequest> request)
        {
            var grpcChannel = GrpcChannel.ForAddress("http://localhost:5292");
            var client = new Product.ProductClient(grpcChannel);
            using var streamingCall = client.CreateClientStreamProduct();

            foreach (var data in request)
            {
                await streamingCall.RequestStream.WriteAsync(data);

                await Task.Delay(1000);
            }

            await streamingCall.RequestStream.CompleteAsync();

            return Ok("Success");
        }
    }
}
