using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.Product;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Entites;
using PFS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        //create prdoduct
        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            // Check category exists
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.IsActive);

            if (category == null)
                throw new NotFoundException("Category not found");

            // Check duplicate product name
            var exists = await _context.Products
                .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new AlreadyExisitException("Product already exists");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            return MapToResponseDto(product);
        }

        //update product
        public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Product not found");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId && c.IsActive);

            if (!categoryExists)
                throw new NotFoundException("Category not found");

            var duplicate = await _context.Products
                .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower() && p.Id != id);

            if (duplicate)
                throw new AlreadyExisitException("Another product with same name already exists");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;
            product.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            return MapToResponseDto(product);
        }


        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Product not found");

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }


        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return products.Select(MapToResponseDto).ToList();
        }
        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && p.Category.IsActive);
            if (product == null)
                throw new NotFoundException("Product not found");

            return MapToResponseDto(product);
        }
        public async Task<List<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == categoryId && c.IsActive);

            if (!categoryExists)
                throw new NotFoundException("Category not found or InActive !");

            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category) 
                .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.IsActive)
                .ToListAsync();

            return products.Select(MapToResponseDto).ToList();
        }
        public async Task<List<ProductResponseDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive && p.Name.ToLower()
                .Contains(searchTerm.ToLower()))
                .ToListAsync();

            return products.Select(MapToResponseDto).ToList();
        }
        public async Task<List<ProductResponseDto>> GetPaginatedProductsAsync(int pageNo, int pageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
                throw new BadRequestException("Invalid pagination values");

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products.Select(MapToResponseDto).ToList();
        }

        private ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}
