using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using EventSourceUI.Dtos;
using MediatR;

namespace EventSourceUI.Queries
{
    public class GetProductAllListByUserId : IRequest<List<ProductDto>>, IRequest<Unit>
    {
        public int UserId { get; set; }
    }
}