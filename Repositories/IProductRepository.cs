using ProductAPI.DTOs;
using ProductAPI.Models;
using ProductAPI.Wrappers;

namespace ProductAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<PagedResult<Product>> GetPagedAsync(ProductQueryParameters parameters);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetByBrandAsync(string brand);
        Task<int> GetCountAsync();
        Task<bool> SaveChangesAsync();
    }
}
