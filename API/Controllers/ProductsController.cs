using Case_1_2.Core.Application.DTOs;
using Case_1_2.Core.Application.Queries.Products;
using Case_1_2.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Case_1_2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService, 
            IMediator mediator,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                _logger.LogInformation("Getting all products via CQRS");
                var query = new GetAllProductsQuery();
                var products = await _mediator.Send(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDto>> GetProduct([Required] int id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId} via CQRS", id);
                var query = new GetProductByIdQuery(id);
                var product = await _mediator.Send(query);
                
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating new product: {ProductName}", createProductDto.Name);
                var product = await _productService.CreateProductAsync(createProductDto);
                
                return CreatedAtAction(
                    nameof(GetProduct), 
                    new { id = product.Id }, 
                    product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}", createProductDto.Name);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductDto>> UpdateProduct([Required] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating product with ID: {ProductId}", id);
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteProduct([Required] int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                var result = await _productService.DeleteProductAsync(id);
                
                if (!result)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return NotFound($"Product with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
