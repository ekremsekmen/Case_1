using Case_1_2.Core.Application.DTOs;
using Case_1_2.Core.Application.Queries.Products;
using Case_1_2.Infrastructure.Services;
using MediatR;

namespace Case_1_2.Core.Application.Handlers.Products
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductService _productService;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(
            IProductService productService,
            ILogger<GetAllProductsQueryHandler> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetAllProductsQuery with UseCache: {UseCache}", request.UseCache);
                
                var products = await _productService.GetAllProductsAsync();
                
                _logger.LogInformation("Successfully retrieved {Count} products", products.Count());
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAllProductsQuery");
                throw;
            }
        }
    }
}
