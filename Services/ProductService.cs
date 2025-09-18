using Case_1.DTOs;
using Case_1.Models;
using Case_1.Repositories;

namespace Case_1.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
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
            return MapToDto(createdProduct);
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
            return updatedProduct == null ? null : MapToDto(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
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
