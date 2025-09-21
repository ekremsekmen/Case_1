using Case_1.Core.Application.DTOs;
using MediatR;

namespace Case_1.Core.Application.Commands.Products
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Category { get; set; }
    }
}
