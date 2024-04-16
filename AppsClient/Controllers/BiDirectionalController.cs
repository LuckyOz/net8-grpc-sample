
using AppsClient.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Channels;

namespace AppsClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiDirectionalController(ILogger<BiDirectionalController> logger) : ControllerBase
    {
        [HttpPost("get_create_product_bidirectional_stream")]
        public async Task GetCreateProductBidirectionalStream(List<ProductAddRequest> requests)
        {
            var grpcChannel = GrpcChannel.ForAddress("http://localhost:5292");
            var client = new Product.ProductClient(grpcChannel);
            using var streamingCall = client.CreateGetStreamProduct();

            var responseReaderTask = Task.Run(async () =>
            {
                await foreach (var response in streamingCall.ResponseStream.ReadAllAsync())
                {
                    logger.LogInformation(JsonConvert.SerializeObject(response));
                }
            });

            foreach (var data in requests)
            {
                await streamingCall.RequestStream.WriteAsync(data);
            }

            await streamingCall.RequestStream.CompleteAsync();
            await responseReaderTask;
        }
    }
}
