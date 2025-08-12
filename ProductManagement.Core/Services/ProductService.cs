using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;

namespace ProductManagement.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync(); // return all products
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id); // return product by ID
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedDate = DateTime.Now;
            return await _productRepository.CreateAsync(product); // create the product save the DateTime
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id); // get existing ID

            // FIXED: Changed logic - throw exception when product is NOT found (null)
            if (existingProduct == null)
            {
                throw new Exception($"Product with ID {id} not found");
            }

            product.Id = id;
            product.CreatedDate = existingProduct.CreatedDate; // preserve original creation date
            product.UpdatedDate = DateTime.UtcNow; // set update timestamp

            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
                throw new Exception($"Product with ID {id} not found");

            await _productRepository.DeleteAsync(id);
        }

        public async Task<bool> IsProductAvailableAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null && product.StockQuantity >= quantity;
        }
    }
}