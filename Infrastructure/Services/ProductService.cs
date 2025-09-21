using Case_1_2.Core.Application.DTOs;
using Case_1_2.Core.Domain.Entities;
using Case_1_2.Infrastructure.Repositories;

namespace Case_1_2.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductService> _logger;

        // Cache keys
        private const string PRODUCTS_CACHE_KEY = "products:all";
        private const string PRODUCT_CACHE_KEY_PREFIX = "products:";
        private const string PRODUCT_CACHE_PATTERN = "products:*";

        public ProductService(
            IProductRepository productRepository,
            ICacheService cacheService,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Try to get from cache first
            var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(PRODUCTS_CACHE_KEY);
            if (cachedProducts != null)
            {
                _logger.LogInformation("Products retrieved from cache");
                return cachedProducts;
            }

            // If not in cache, get from database
            _logger.LogInformation("Products not found in cache, retrieving from database");
            var products = await _productRepository.GetAllAsync();
            var productDtos = products.Select(MapToDto).ToList();

            // Cache the results for 5 minutes
            await _cacheService.SetAsync(PRODUCTS_CACHE_KEY, productDtos, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Products cached for 5 minutes");

            return productDtos;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var cacheKey = $"{PRODUCT_CACHE_KEY_PREFIX}{id}";
            
            // Try to get from cache first
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                _logger.LogInformation("Product {ProductId} retrieved from cache", id);
                return cachedProduct;
            }

            // If not in cache, get from database
            _logger.LogInformation("Product {ProductId} not found in cache, retrieving from database", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            var productDto = MapToDto(product);
            
            // Cache the result for 10 minutes
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Product {ProductId} cached for 10 minutes", id);

            return productDto;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                Category = createProductDto.Category
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            var productDto = MapToDto(createdProduct);

            // Cache invalidation: Remove all products cache since list changed
            await InvalidateProductCaches();
            _logger.LogInformation("Product created and cache invalidated");

            return productDto;
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateProductDto.Name))
                existingProduct.Name = updateProductDto.Name;
            
            if (updateProductDto.Description != null)
                existingProduct.Description = updateProductDto.Description;
            
            if (updateProductDto.Price.HasValue)
                existingProduct.Price = updateProductDto.Price.Value;
            
            if (updateProductDto.Stock.HasValue)
                existingProduct.Stock = updateProductDto.Stock.Value;
            
            if (updateProductDto.Category != null)
                existingProduct.Category = updateProductDto.Category;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            if (updatedProduct == null) 
                return null;

            var productDto = MapToDto(updatedProduct);

            // Cache invalidation: Remove specific product cache and all products cache
            await InvalidateProductCaches(id);
            _logger.LogInformation("Product {ProductId} updated and cache invalidated", id);

            return productDto;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _productRepository.DeleteAsync(id);
            
            if (result)
            {
                // Cache invalidation: Remove specific product cache and all products cache
                await InvalidateProductCaches(id);
                _logger.LogInformation("Product {ProductId} deleted and cache invalidated", id);
            }

            return result;
        }

        private async Task InvalidateProductCaches(int? specificProductId = null)
        {
            try
            {
                // Remove all products cache
                await _cacheService.RemoveAsync(PRODUCTS_CACHE_KEY);

                if (specificProductId.HasValue)
                {
                    // Remove specific product cache
                    var specificCacheKey = $"{PRODUCT_CACHE_KEY_PREFIX}{specificProductId}";
                    await _cacheService.RemoveAsync(specificCacheKey);
                }
                else
                {
                    // Remove all product-related caches using pattern
                    await _cacheService.RemovePatternAsync(PRODUCT_CACHE_PATTERN);
                }

                _logger.LogDebug("Product caches invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating product caches");
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
