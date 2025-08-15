using AutoMapper;
using ProductAPI.DTOs;
using ProductAPI.Models;
using ProductAPI.Repositories;
using ProductAPI.Wrappers;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all products");

                var products = await _productRepository.GetAllAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

                _logger.LogInformation("Successfully retrieved {Count} products", productDtos.Count());

                return ApiResponse<IEnumerable<ProductResponseDto>>.SuccessResponse(
                    productDtos,
                    $"Successfully retrieved {productDtos.Count()} products");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products");
                return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                    "An error occurred while retrieving products");
            }
        }

        public async Task<ApiResponse<PagedResult<ProductResponseDto>>> GetPagedProductsAsync(ProductQueryParameters parameters)
        {
            try
            {
                _logger.LogInformation("Getting paged products with parameters: {@Parameters}", parameters);

                // Validate parameters
                if (parameters.PageNumber < 1)
                    parameters.PageNumber = 1;

                if (parameters.PageSize < 1 || parameters.PageSize > 100)
                    parameters.PageSize = 10;

                var pagedResult = await _productRepository.GetPagedAsync(parameters);
                var productDtos = _mapper.Map<List<ProductResponseDto>>(pagedResult.Items);

                var result = new PagedResult<ProductResponseDto>
                {
                    Items = productDtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };

                _logger.LogInformation("Successfully retrieved page {PageNumber} with {Count} products",
                    parameters.PageNumber, productDtos.Count);

                return ApiResponse<PagedResult<ProductResponseDto>>.SuccessResponse(
                    result,
                    $"Successfully retrieved page {parameters.PageNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged products");
                return ApiResponse<PagedResult<ProductResponseDto>>.ErrorResponse(
                    "An error occurred while retrieving paged products");
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid product ID provided: {ProductId}", id);
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        "Invalid product ID",
                        new List<string> { "Product ID must be greater than 0" });
                }

                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", id);
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Product with ID {id} not found");
                }

                var productDto = _mapper.Map<ProductResponseDto>(product);

                _logger.LogInformation("Successfully retrieved product: {ProductName}", product.Name);

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    productDto,
                    "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product with ID: {ProductId}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "An error occurred while retrieving the product");
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(ProductCreateDto productCreateDto)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", productCreateDto.Name);

                var product = _mapper.Map<Product>(productCreateDto);
                var createdProduct = await _productRepository.CreateAsync(product);
                var productDto = _mapper.Map<ProductResponseDto>(createdProduct);

                _logger.LogInformation("Successfully created product with ID: {ProductId}", createdProduct.Id);

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    productDto,
                    "Product created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}", productCreateDto.Name);
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "An error occurred while creating the product");
            }
        }

        public async Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, ProductUpdateDto productUpdateDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid product ID provided for update: {ProductId}", id);
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        "Invalid product ID",
                        new List<string> { "Product ID must be greater than 0" });
                }

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product not found for update with ID: {ProductId}", id);
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        $"Product with ID {id} not found");
                }

                var updatedProduct = _mapper.Map<Product>(productUpdateDto);
                updatedProduct.Id = id;
                updatedProduct.CreatedAt = existingProduct.CreatedAt; // Preserve original creation date

                var result = await _productRepository.UpdateAsync(updatedProduct);
                if (result == null)
                {
                    _logger.LogError("Failed to update product with ID: {ProductId}", id);
                    return ApiResponse<ProductResponseDto>.ErrorResponse(
                        "Failed to update product");
                }

                var productDto = _mapper.Map<ProductResponseDto>(result);

                _logger.LogInformation("Successfully updated product: {ProductName}", result.Name);

                return ApiResponse<ProductResponseDto>.SuccessResponse(
                    productDto,
                    "Product updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", id);
                return ApiResponse<ProductResponseDto>.ErrorResponse(
                    "An error occurred while updating the product");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Invalid product ID provided for deletion: {ProductId}", id);
                    return ApiResponse<bool>.ErrorResponse(
                        "Invalid product ID",
                        new List<string> { "Product ID must be greater than 0" });
                }

                var exists = await _productRepository.ExistsAsync(id);
                if (!exists)
                {
                    _logger.LogWarning("Product not found for deletion with ID: {ProductId}", id);
                    return ApiResponse<bool>.ErrorResponse(
                        $"Product with ID {id} not found");
                }

                var result = await _productRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
                    return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully");
                }
                else
                {
                    _logger.LogError("Failed to delete product with ID: {ProductId}", id);
                    return ApiResponse<bool>.ErrorResponse("Failed to delete product");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the product");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponseDto>>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogWarning("Empty search term provided");
                    return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                        "Search term cannot be empty",
                        new List<string> { "Please provide a valid search term" });
                }

                var products = await _productRepository.SearchAsync(searchTerm);
                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

                _logger.LogInformation("Search completed. Found {Count} products for term: {SearchTerm}",
                    productDtos.Count(), searchTerm);

                return ApiResponse<IEnumerable<ProductResponseDto>>.SuccessResponse(
                    productDtos,
                    $"Found {productDtos.Count()} products matching '{searchTerm}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching products with term: {SearchTerm}", searchTerm);
                return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                    "An error occurred while searching products");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetProductsByCategoryAsync(string category)
        {
            try
            {
                _logger.LogInformation("Getting products by category: {Category}", category);

                if (string.IsNullOrWhiteSpace(category))
                {
                    _logger.LogWarning("Empty category provided");
                    return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                        "Category cannot be empty");
                }

                var products = await _productRepository.GetByCategoryAsync(category);
                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

                _logger.LogInformation("Successfully retrieved {Count} products for category: {Category}",
                    productDtos.Count(), category);

                return ApiResponse<IEnumerable<ProductResponseDto>>.SuccessResponse(
                    productDtos,
                    $"Found {productDtos.Count()} products in category '{category}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products by category: {Category}", category);
                return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                    "An error occurred while retrieving products by category");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetProductsByBrandAsync(string brand)
        {
            try
            {
                _logger.LogInformation("Getting products by brand: {Brand}", brand);

                if (string.IsNullOrWhiteSpace(brand))
                {
                    _logger.LogWarning("Empty brand provided");
                    return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                        "Brand cannot be empty");
                }

                var products = await _productRepository.GetByBrandAsync(brand);
                var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

                _logger.LogInformation("Successfully retrieved {Count} products for brand: {Brand}",
                    productDtos.Count(), brand);

                return ApiResponse<IEnumerable<ProductResponseDto>>.SuccessResponse(
                    productDtos,
                    $"Found {productDtos.Count()} products from brand '{brand}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products by brand: {Brand}", brand);
                return ApiResponse<IEnumerable<ProductResponseDto>>.ErrorResponse(
                    "An error occurred while retrieving products by brand");
            }
        }

        public async Task<ApiResponse<int>> GetProductCountAsync()
        {
            try
            {
                _logger.LogInformation("Getting total product count");

                var count = await _productRepository.GetCountAsync();

                _logger.LogInformation("Total product count: {Count}", count);

                return ApiResponse<int>.SuccessResponse(count, $"Total products: {count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product count");
                return ApiResponse<int>.ErrorResponse(
                    "An error occurred while retrieving product count");
            }
        }
    }
}
