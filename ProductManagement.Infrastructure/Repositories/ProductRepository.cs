using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;
using System.Data;

namespace ProductManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT ID, NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, 
                                   CREATED_DATE, UPDATED_DATE 
                            FROM PRODUCTS 
                            ORDER BY ID";

                using var command = new OracleCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapToProduct(reader));
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                throw;
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"SELECT ID, NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, 
                                   CREATED_DATE, UPDATED_DATE 
                            FROM PRODUCTS 
                            WHERE ID = :id";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("id", id));

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToProduct(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                throw;
            }

            return null;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"INSERT INTO PRODUCTS (NAME, DESCRIPTION, PRICE, STOCK_QUANTITY, CREATED_DATE) 
                            VALUES (:name, :description, :price, :stockQuantity, :createdDate)
                            RETURNING ID INTO :id";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("name", product.Name));
                command.Parameters.Add(new OracleParameter("description", product.Description));
                command.Parameters.Add(new OracleParameter("price", product.Price));
                command.Parameters.Add(new OracleParameter("stockQuantity", product.StockQuantity));
                command.Parameters.Add(new OracleParameter("createdDate", product.CreatedDate));

                var idParam = new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output);
                command.Parameters.Add(idParam);

                await command.ExecuteNonQueryAsync();

                product.Id = Convert.ToInt32(idParam.Value.ToString());
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"UPDATE PRODUCTS 
                            SET NAME = :name, DESCRIPTION = :description, 
                                PRICE = :price, STOCK_QUANTITY = :stockQuantity, 
                                UPDATED_DATE = :updatedDate 
                            WHERE ID = :id";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("name", product.Name));
                command.Parameters.Add(new OracleParameter("description", product.Description));
                command.Parameters.Add(new OracleParameter("price", product.Price));
                command.Parameters.Add(new OracleParameter("stockQuantity", product.StockQuantity));
                command.Parameters.Add(new OracleParameter("updatedDate", product.UpdatedDate));
                command.Parameters.Add(new OracleParameter("id", product.Id));

                await command.ExecuteNonQueryAsync();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = "DELETE FROM PRODUCTS WHERE ID = :id";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("id", id));

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(1) FROM PRODUCTS WHERE ID = :id";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("id", id));

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExistsAsync: {ex.Message}");
                throw;
            }
        }

        private static Product MapToProduct(OracleDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32("ID"),
                Name = reader.GetString("NAME"),
                Description = reader.GetString("DESCRIPTION"),
                Price = reader.GetDecimal("PRICE"),
                StockQuantity = reader.GetInt32("STOCK_QUANTITY"),
                CreatedDate = reader.GetDateTime("CREATED_DATE"),
                UpdatedDate = reader.IsDBNull("UPDATED_DATE") ? null : reader.GetDateTime("UPDATED_DATE")
            };
        }
    }
}