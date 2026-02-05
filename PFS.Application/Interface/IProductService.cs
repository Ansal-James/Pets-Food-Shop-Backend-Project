using PFS.Application.DTOs.Product;
using PFS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task DeleteProductAsync(Guid id);
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync(Guid id);
        Task<List<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId);
        Task<List<ProductResponseDto>> SearchProductsAsync(string searchTerm);

        // Pagination
        Task<List<ProductResponseDto>> GetPaginatedProductsAsync(int pageNo, int pageSize);
    }
}
