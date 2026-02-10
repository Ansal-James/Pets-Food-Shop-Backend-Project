using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.Category;
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
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
            if(exists)
                throw new AlreadyExisitException("Category with the same name already exists.");

            var cate = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Categories.Add(cate);
            await _context.SaveChangesAsync();

            return MapToResponseDto(cate); 
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("Category not found");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToResponseDto).ToList();
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
        {
            var cate = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
            if(cate == null)
                throw new NotFoundException("Category not found...");

            return MapToResponseDto(cate);
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var cate = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cate == null)
                throw new NotFoundException("Category not found");

            var duplicate = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);

            if (duplicate)
                throw new AlreadyExisitException("Another category with the same name already exists.");

            cate.Name = dto.Name;
            cate.Description = dto.Description;
            cate.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return MapToResponseDto(cate);
        }

        private static CategoryResponseDto MapToResponseDto(Category cate)
        {
            return new CategoryResponseDto
            {
                Id = cate.Id,
                Name = cate.Name,
                Description = cate.Description,
                IsActive = cate.IsActive,
                CreatedAt = cate.CreatedAt
            };
        }
    }
}
