using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourceUI.Dtos;
using EventSourceUI.Models;
using EventSourceUI.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourceUI.Handler
{
    public class GetProductAllListByUserIdHandler : IRequestHandler<GetProductAllListByUserId, List<ProductDto>>
    {
        private readonly AppDbContext _dbContext;

        public GetProductAllListByUserIdHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductDto>> Handle(GetProductAllListByUserId request,
            CancellationToken cancellationToken)
        {
            var products = await  _dbContext.Products.Where(x => x.UserId == request.UserId)
                .Select(a => new ProductDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Price = a.Price,
                    Stock = a.Stock,
                    UserId = a.UserId
                }).ToListAsync();

            return products;
        }
    }
}