using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.DTOs;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var response = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductCreateDto createDto)
        {
            var product = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                StockQuantity = createDto.StockQuantity
            };

            var createdProduct = await _productService.CreateProductAsync(product);

            var response = new ProductResponseDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                CreatedDate = createdProduct.CreatedDate,
                UpdatedDate = createdProduct.UpdatedDate
            };

            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, ProductUpdateDto updateDto)
        {
            try
            {
                var product = new Product
                {
                    Name = updateDto.Name,
                    Description = updateDto.Description,
                    Price = updateDto.Price,
                    StockQuantity = updateDto.StockQuantity
                };

                var updatedProduct = await _productService.UpdateProductAsync(id, product);

                var response = new ProductResponseDto
                {
                    Id = updatedProduct.Id,
                    Name = updatedProduct.Name,
                    Description = updatedProduct.Description,
                    Price = updatedProduct.Price,
                    StockQuantity = updatedProduct.StockQuantity,
                    CreatedDate = updatedProduct.CreatedDate,
                    UpdatedDate = updatedProduct.UpdatedDate
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/availability/{quantity}")]
        public async Task<ActionResult<bool>> CheckAvailability(int id, int quantity)
        {
            var isAvailable = await _productService.IsProductAvailableAsync(id, quantity);
            return Ok(isAvailable);
        }
    }
}