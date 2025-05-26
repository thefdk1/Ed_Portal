using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = _categoryRepository.GetAll().ToList();
            
            var categoryDtos = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedDate = c.CreatedDate
            }).ToList();

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdWithCoursesAsync(id);
            
            if (category == null)
            {
                return NotFound(new { message = "Kategori bulunamadı." });
            }

            var categoryDto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate,
                Courses = category.Courses?.Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    YoutubeLink = c.YoutubeLink,
                    IsActive = c.IsActive,
                    CreatedDate = c.CreatedDate
                }).ToList()
            };

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDTO createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                IsActive = true
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDTO updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return NotFound(new { message = "Kategori bulunamadı." });
            }

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.IsActive = updateCategoryDto.IsActive;
            category.UpdatedDate = DateTime.Now;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();

            return Ok(new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return NotFound(new { message = "Kategori bulunamadı." });
            }

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();

            return NoContent();
        }
    }
} 