using Case_1.Core.Application.DTOs;
using MediatR;

namespace Case_1.Core.Application.Queries.Products
{
    public class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public int Id { get; set; }
        public bool UseCache { get; set; } = true;
        
        public GetProductByIdQuery(int id, bool useCache = true)
        {
            Id = id;
            UseCache = useCache;
        }
    }
}
