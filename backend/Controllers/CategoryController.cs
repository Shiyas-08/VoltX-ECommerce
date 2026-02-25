using ECommerce.DTOs;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

       
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult Create(CreateCategoryDto dto)
        {
            var category = _service.Create(dto.Name);

            if (category == null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Category already exists",
                    StatusCodes.Status400BadRequest
                ));
            }

            var response = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };

            return Ok(ApiResponse<CategoryResponseDto>.Ok(
                response,
                "Category created successfully",
                StatusCodes.Status200OK
            ));


        }
        //get all category

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _service.GetAll()
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive
                })
                .ToList();

            return Ok(ApiResponse<List<CategoryResponseDto>>.Ok(
                categories,
                "Categories fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        // get by id 
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _service.GetById(id);
            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Category not found",
                    StatusCodes.Status404NotFound
                ));
            }

            var response = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };

            return Ok(ApiResponse<CategoryResponseDto>.Ok(
                response,
                "Category fetched successfully",
                StatusCodes.Status200OK
            ));
        }

        // update
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateCategoryDto dto)
        {
            var category = _service.Update(id, dto);
            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Category not found",
                    StatusCodes.Status404NotFound
                ));
            }

            var response = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };

            return Ok(ApiResponse<CategoryResponseDto>.Ok(
                response,
                "Category updated successfully",
                StatusCodes.Status200OK
            ));
        }

    


        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/toggle")]
        public IActionResult Toggle(int id)
        {
            var result = _service.ToggleStatus(id);

            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(
                    "Category not found",
                    StatusCodes.Status404NotFound
                ));
            }

            return Ok(ApiResponse<string>.Ok(
                null,
                "Category status toggled successfully",
                StatusCodes.Status200OK
            ));
        }


    }
}
