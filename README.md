# Product Management API

A simple .NET Core Web API for managing products using Clean Architecture.

## Features

- Create, Read, Update, Delete products
- Check stock availability
- Clean Architecture with 3 layers
- Swagger documentation

## Product Properties

- **Id**: Auto-generated number
- **Name**: Product name
- **Description**: Product details  
- **Price**: Product price
- **StockQuantity**: Available stock
- **CreatedDate**: When created
- **UpdatedDate**: When last modified

## API Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/product` | Get all products |
| GET | `/api/product/{id}` | Get one product |
| POST | `/api/product` | Create product |
| PUT | `/api/product/{id}` | Update product |
| DELETE | `/api/product/{id}` | Delete product |

4. **Run**
```bash
dotnet run --project ProductManagement.API
```

## Project Structure

```
ProductManagement/
├── ProductManagement.API/          # Controllers, DTOs
├── ProductManagement.Core/         # Business logic, Models
└── ProductManagement.Infrastructure/   # Database, Repositories
```

## Test API

Go to: `https://localhost:7001/swagger`

## Sample Request

**Create Product:**
```json
POST /api/product
{
  "name": "Smartphone",
  "description": "Latest smartphone",
  "price": 899.99,
  "stockQuantity": 25
}
```

**Response:**
```json
{
  "id": 1,
  "name": "Smartphone",
  "description": "Latest smartphone",
  "price": 899.99,
  "stockQuantity": 25,
  "createdDate": "2025-12-08T11:00:43.347"
}
```
<img width="1206" height="222" alt="image" src="https://github.com/user-attachments/assets/8c49e00f-751d-4b6a-8dae-40bce24df980" />
