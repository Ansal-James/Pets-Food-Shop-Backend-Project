using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.Application.DTOs.Product;
using PFS.Application.Interface;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        // Create product Admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return Ok(result);
        }

        // Update product Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(id, dto);
            return Ok(result);
        }

        // Delete product Admin only - Soft delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Product deleted successfully" });
        }

        

        // Get all products
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        // Get product by id
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return Ok(result);
        }

        // Get products by category
        [AllowAnonymous]
        [HttpGet("category/{categoryId:guid}")]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }

        // Search products
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var result = await _productService.SearchProductsAsync(query);
            return Ok(result);
        }

        // Get paginated products
        [AllowAnonymous]
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedProducts(
            [FromQuery] int pageNo,
            [FromQuery] int pageSize)
        {
            var result = await _productService.GetPaginatedProductsAsync(pageNo, pageSize);
            return Ok(result);
        }
    }
}
