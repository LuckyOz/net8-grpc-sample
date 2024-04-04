
using Grpc.Net.Client;
using AppsClient.Protos;
using Microsoft.AspNetCore.Mvc;

namespace AppsClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnaryController : ControllerBase
    {
        [HttpGet("get_all_product")]
        public async Task<IActionResult> GetAllProduct()
        {
            var data = new ProductAllRequest { Page = 1, PageLimit = 10 };
            var grpcChannel = GrpcChannel.ForAddress("http://localhost:5292");
            var client = new Product.ProductClient(grpcChannel);
            return Ok(await client.GetAllProductAsync(data));
        }
    }
}
