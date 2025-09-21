using Case_1_2.Core.Application.DTOs;
using Case_1_2.Core.Application.Queries.Products;
using Case_1_2.Infrastructure.Services;
using MediatR;

namespace Case_1_2.Core.Application.Handlers.Products
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductService _productService;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IProductService productService,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetProductByIdQuery for Id: {ProductId} with UseCache: {UseCache}", 
                    request.Id, request.UseCache);
                
                var product = await _productService.GetProductByIdAsync(request.Id);
                
                if (product == null)
                {
                    _logger.LogWarning("Product with Id {ProductId} not found", request.Id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved product with Id: {ProductId}", request.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetProductByIdQuery for Id: {ProductId}", request.Id);
                throw;
            }
        }
    }
}
