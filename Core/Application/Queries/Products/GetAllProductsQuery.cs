using Case_1_2.Core.Application.DTOs;
using MediatR;

namespace Case_1_2.Core.Application.Queries.Products
{
    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public bool UseCache { get; set; } = true;
    }
}
