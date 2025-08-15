using ProductAPI.DTOs;
using ProductAPI.Wrappers;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetAllProductsAsync();
        Task<ApiResponse<PagedResult<ProductResponseDto>>> GetPagedProductsAsync(ProductQueryParameters parameters);
        Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto productCreateDto);
        Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductUpdateDto productUpdateDto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<ApiResponse<IEnumerable<ProductResponseDto>>> SearchProductsAsync(string searchTerm);
        Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetProductsByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetProductsByBrandAsync(string brand);
        Task<ApiResponse<int>> GetProductCountAsync();
    }
}
