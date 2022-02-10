using System;
using System.Threading.Tasks;
using EventSourceUI.Commands;
using EventSourceUI.Dtos;
using EventSourceUI.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EventSourceUI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController (IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto productDto)
        {
            await _mediator.Send(new CreateProductCommand()
            {
                CreateProductDto = productDto
            });

            return  NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand()
            {
                Id = id
            });

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeProductName(ChangeProductNameDto productNameDto)
        {
            await _mediator.Send(new ChangeProductNameCommand()
            {
                ChangeProductNameDto = productNameDto
            });

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeProductPrice(ChangeProductPriceDto productPriceDto)
        {
            await _mediator.Send(new ChangeProductPriceCommand()
            {
                ChangeProductPriceDto = productPriceDto
            });

            return NoContent();
        }

        public async Task<IActionResult> GetAllListByUserId(int userId)
        {
           var result= await  _mediator.Send(new GetProductAllListByUserId()
            {
                UserId = userId
            });

           return Ok(result);
        }
    }
}