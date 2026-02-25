using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [AllowAnonymous]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // get all products
        [AllowAnonymous]

        [HttpGet("User")]
        public IActionResult GetAll()
        {
            var products = _service.UserGetAll();

            var response = products.Select(p => new ProductCreateResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.CategoryId,
                Stock = p.Stock,
                IsFeatured = p.IsFeatured,
                ImageUrls = p.Images
                .Select(i => i.ImageUrl)
                .ToList()

            }).ToList();

            return Ok(ApiResponse<List<ProductCreateResponseDto>>.Ok(
                response,
                "Products fetched successfully",
                StatusCodes.Status200OK
            ));
        }


        //admin get all
        [Authorize(Policy = "AdminOnly")]

        [HttpGet("Admin")]
        public IActionResult AdminGetAll()
        {
            var products = _service.AdminGetAll();

            var response = products.Select(p => new AdminProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Stock = p.Stock,
                IsFeatured = p.IsFeatured,
                IsActive = p.IsActive,
                Images = p.Images
                    .Where(i => i.IsActive)
                    .Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl
                    })
                    .ToList()
            }).ToList();

            return Ok(ApiResponse<List<AdminProductResponseDto>>.Ok(
                response,
                "Admin products fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _service.GetById(id);

            if (product == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound
                ));
            }

            var response = new ProductCreateResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Stock = product.Stock,
                IsFeatured = product.IsFeatured,

                ImageUrls = product.Images
             .Select(i => i.ImageUrl)
                .ToList()

            };

            return Ok(ApiResponse<ProductCreateResponseDto>.Ok(
                response,
                "Product fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        //crreate products 

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var (product, error) = await _service.CreateAsync(dto);

            if (error != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    error,
                    StatusCodes.Status400BadRequest
                ));
            }

            var response = new ProductCreateResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Stock = product.Stock,
                IsFeatured = product.IsFeatured,
                ImageUrls = product.Images.Select(i => i.ImageUrl).ToList()
            };

            return Ok(ApiResponse<ProductCreateResponseDto>.Ok(
                response,
                "Product created successfully",
                StatusCodes.Status200OK
            ));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            var (updated, error) = await _service.UpdateAsync(id, dto);

            if (error != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    error,
                    StatusCodes.Status400BadRequest
                ));
            }

            var response = new ProductUpdateResponseDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Price = updated.Price,
                Description = updated.Description,
                CategoryId = updated.CategoryId,
                Stock = updated.Stock,
                IsFeatured = updated.IsFeatured
            };

            return Ok(ApiResponse<ProductUpdateResponseDto>.Ok(
                response,
                "Product updated successfully",
                StatusCodes.Status200OK
            ));
        }


        //pagination
        [AllowAnonymous]

        [HttpGet("Page")]
        public IActionResult GetAll([FromQuery] ProductQueryDto query)
        {
            if (query.PageNumber <= 0 || query.PageSize <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "PageNumber and PageSize must be greater than 0",
                    StatusCodes.Status400BadRequest
                ));
            }

            var result = _service.GetPagedProducts(query);

            return Ok(ApiResponse<PagedResultDto<ProductCreateResponseDto>>.Ok(
                result,
                "Products fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        [HttpGet("Search")]
        public IActionResult Search([FromQuery] ProductSearchQueryDto query)
        {
            if (string.IsNullOrWhiteSpace(query.Search))
            {
                return BadRequest(ApiResponse<string>.Fail(
                    "Search keyword is required",
                    StatusCodes.Status400BadRequest
                ));
            }

            if (query.PageNumber <= 0 || query.PageSize <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "PageNumber and PageSize must be greater than 0",
                    StatusCodes.Status400BadRequest
                ));
            }

            var result = _service.SearchProducts(query);

            return Ok(ApiResponse<PagedResultDto<ProductCreateResponseDto>>.Ok(
                result,
                result.Items.Count == 0 ? "No products found" : "Search results fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        // filter
            [AllowAnonymous]

        [HttpGet("filter")]
        public IActionResult Filter([FromQuery] ProductFilterDto filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "PageNumber and PageSize must be greater than 0",
                    StatusCodes.Status400BadRequest
                ));
            }

            var result = _service.FilterProducts(filter);

            return Ok(ApiResponse<PagedResultDto<ProductCreateResponseDto>>.Ok(
                result,
                result.Items.Count == 0 ? "No products found" : "Filtered products fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        // update stock

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/stock")]
        public IActionResult UpdateStock(int id, UpdateStockDto dto)
        {
            try
            {
                _service.UpdateStock(id, dto.Quantity);

                return Ok(ApiResponse<string>.Ok(
                    null,
                    "Stock updated successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ));
            }
        }

        //delete 
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.SoftDeleteAsync(id);

            if (!deleted)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound
                ));
            }

            return Ok(ApiResponse<string>.Ok(
                null,
                "Product deactivated successfully",
                StatusCodes.Status200OK
            ));
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/toggle")]
        public IActionResult ToggleProduct(int id)
        {
            var result = _service.ToggleProduct(id);

            if (!result)
                return NotFound(ApiResponse<string>.Fail(
                    "Product not found",
                    StatusCodes.Status404NotFound
                ));

            return Ok(ApiResponse<string>.Ok(
                null,
                "Product status toggled successfully",
                StatusCodes.Status200OK
            ));
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddImage(int id, IFormFile image)
        {
            var imageUrl = await _service.AddProductImageAsync(id, image);

            return Ok(ApiResponse<string>.Ok(
                imageUrl,
                "Image added successfully",
                StatusCodes.Status200OK
            ));
        }


        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> RemoveProductImage(int imageId)
        {
            var result = await _service.RemoveProductImage(imageId);

            if (!result)
                return NotFound(new { IsSuccess = false, Message = "Image not found" });

            return Ok(new
            {
                IsSuccess = true,
                Message = "Image removed successfully"
            });
        }
            
    }
}