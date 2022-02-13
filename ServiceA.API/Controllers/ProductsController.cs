using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController (ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data =await _productService.GetProduct(id);

            return Ok(data);
        }
    }
}