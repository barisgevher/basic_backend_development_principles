using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Services;
using ProductAPI.Wrappers;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all active products</returns>
        /// <response code="200">Returns the list of products</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetAllProducts()
        {
            _logger.LogInformation("GET /api/products - Getting all products");

            var response = await _productService.GetAllProductsAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get products with pagination and filtering
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of products</returns>
        /// <response code="200">Returns the paginated list of products</response>
        /// <response code="400">Bad request - invalid parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> GetPagedProducts([FromQuery] ProductQueryParameters parameters)
        {
            _logger.LogInformation("GET /api/products/paged - Getting paged products with parameters: {@Parameters}", parameters);

            var response = await _productService.GetPagedProductsAsync(parameters);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Returns the product</response>
        /// <response code="400">Bad request - invalid ID</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProductById([FromRoute] int id)
        {
            _logger.LogInformation("GET /api/products/{ProductId} - Getting product by ID", id);

            var response = await _productService.GetProductByIdAsync(id);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }

            if (response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="productCreateDto">Product creation data</param>
        /// <returns>Created product</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Bad request - invalid data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct([FromBody] ProductCreateDto productCreateDto)
        {
            _logger.LogInformation("POST /api/products - Creating new product: {ProductName}", productCreateDto?.Name);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Model validation failed: {@Errors}", errors);

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed", errors));
            }

            var response = await _productService.CreateProductAsync(productCreateDto);

            if (response.Success)
            {
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = response.Data!.Id },
                    response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="productUpdateDto">Product update data</param>
        /// <returns>Updated product</returns>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Bad request - invalid data</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(
            [FromRoute] int id,
            [FromBody] ProductUpdateDto productUpdateDto)
        {
            _logger.LogInformation("PUT /api/products/{ProductId} - Updating product", id);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Model validation failed: {@Errors}", errors);

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed", errors));
            }

            var response = await _productService.UpdateProductAsync(id, productUpdateDto);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }

            if (response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Delete a product (soft delete)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Deletion status</returns>
        /// <response code="200">Product deleted successfully</response>
        /// <response code="400">Bad request - invalid ID</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct([FromRoute] int id)
        {
            _logger.LogInformation("DELETE /api/products/{ProductId} - Deleting product", id);

            var response = await _productService.DeleteProductAsync(id);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("not found"))
            {
                return NotFound(response);
            }

            if (response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Search products by term
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching products</returns>
        /// <response code="200">Returns the search results</response>
        /// <response code="400">Bad request - invalid search term</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> SearchProducts(
            [FromQuery, Required] string searchTerm)
        {
            _logger.LogInformation("GET /api/products/search - Searching products with term: {SearchTerm}", searchTerm);

            var response = await _productService.SearchProductsAsync(searchTerm);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("empty") || response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>List of products in the category</returns>
        /// <response code="200">Returns the products in category</response>
        /// <response code="400">Bad request - invalid category</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetProductsByCategory([FromRoute] string category)
        {
            _logger.LogInformation("GET /api/products/category/{Category} - Getting products by category", category);

            var response = await _productService.GetProductsByCategoryAsync(category);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("empty") || response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get products by brand
        /// </summary>
        /// <param name="brand">Brand name</param>
        /// <returns>List of products from the brand</returns>
        /// <response code="200">Returns the products from brand</response>
        /// <response code="400">Bad request - invalid brand</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("brand/{brand}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetProductsByBrand([FromRoute] string brand)
        {
            _logger.LogInformation("GET /api/products/brand/{Brand} - Getting products by brand", brand);

            var response = await _productService.GetProductsByBrandAsync(brand);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message.Contains("empty") || response.Message.Contains("Invalid"))
            {
                return BadRequest(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get total product count
        /// </summary>
        /// <returns>Total number of active products</returns>
        /// <response code="200">Returns the product count</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("count")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<int>>> GetProductCount()
        {
            _logger.LogInformation("GET /api/products/count - Getting total product count");

            var response = await _productService.GetProductCountAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}

