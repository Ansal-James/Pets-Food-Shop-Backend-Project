using PFS.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);

        Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);

        Task DeleteCategoryAsync(Guid id);
        Task SoftDeleteCategoryAsync(Guid id);

        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();

        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id);
    }
}
