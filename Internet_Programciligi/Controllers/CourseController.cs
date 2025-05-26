using AutoMapper;
using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseRepository _courseRepository;
        private readonly EnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            CourseRepository courseRepository,
            EnrollmentRepository enrollmentRepository,
            IMapper mapper,
            IWebHostEnvironment hostingEnvironment,
            ILogger<CourseController> logger)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseRepository.GetAllWithEnrollmentsAsync();
            var courseDTOs = _mapper.Map<List<CourseDTO>>(courses);
            return Ok(courseDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _courseRepository.GetWithEnrollmentsAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var courseDTO = _mapper.Map<CourseDTO>(course);
            return Ok(courseDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromForm] string name, [FromForm] string description, [FromForm] int? categoryId, [FromForm] int? teacherProfileId, [FromForm] IFormFile image)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest(new { message = "Kurs adı boş olamaz" });
                }

                var course = new Course
                {
                    Name = name,
                    Description = description ?? "",
                    CategoryId = categoryId,
                    TeacherProfileId = teacherProfileId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // Resim yükleme işlemi
                if (image != null && image.Length > 0)
                {
                    // Resim dosyası uzantısını kontrol et
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        return BadRequest(new { message = "Geçersiz dosya formatı. JPG, PNG veya GIF kullanmalısınız." });
                    }

                    // Dosya boyutu kontrolü (5MB)
                    if (image.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır." });
                    }

                    // Dosyayı kaydetme
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "courses");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Benzersiz dosya adı oluştur
                    var uniqueFileName = $"course_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Dosyayı kaydet
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    course.ImageUrl = $"/uploads/courses/{uniqueFileName}";
                }

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                var courseDTO = _mapper.Map<CourseDTO>(course);
                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, courseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kurs oluşturulurken hata oluştu: {Message}", ex.Message);
                return StatusCode(500, new { message = "Kurs oluşturulurken bir hata oluştu.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] string name, [FromForm] string description, [FromForm] int? categoryId, [FromForm] int? teacherProfileId, [FromForm] bool isActive, [FromForm] IFormFile image)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest(new { message = "Kurs adı boş olamaz" });
                }

                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new { message = "Kurs bulunamadı" });
                }

                // Kurs bilgilerini güncelle
                course.Name = name;
                course.Description = description ?? "";
                course.CategoryId = categoryId;
                course.TeacherProfileId = teacherProfileId;
                course.IsActive = isActive;
                course.UpdatedDate = DateTime.Now;

                // Resim yükleme işlemi
                if (image != null && image.Length > 0)
                {
                    // Resim dosyası uzantısını kontrol et
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        return BadRequest(new { message = "Geçersiz dosya formatı. JPG, PNG veya GIF kullanmalısınız." });
                    }

                    // Dosya boyutu kontrolü (5MB)
                    if (image.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır." });
                    }

                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(course.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, course.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Yeni resmi kaydet
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "courses");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Benzersiz dosya adı oluştur
                    var uniqueFileName = $"course_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Dosyayı kaydet
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    course.ImageUrl = $"/uploads/courses/{uniqueFileName}";
                }

                _courseRepository.Update(course);
                await _courseRepository.SaveChangesAsync();

                var courseDTO = _mapper.Map<CourseDTO>(course);
                return Ok(courseDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kurs güncellenirken hata oluştu: {Message}", ex.Message);
                return StatusCode(500, new { message = "Kurs güncellenirken bir hata oluştu.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _courseRepository.Remove(course);
            await _courseRepository.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseRepository.GetByIdAsync(courseId);
            
            if (course == null)
            {
                return NotFound("Kurs bulunamadı.");
            }

            // Kullanıcının zaten kayıtlı olup olmadığını kontrol et
            var existingEnrollment = await _enrollmentRepository.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
            if (existingEnrollment)
            {
                return BadRequest("Bu kursa zaten kayıtlısınız.");
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                IsActive = true,
                EnrollmentDate = DateTime.Now
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return Ok(new { Message = "Kursa başarıyla kaydoldunuz." });
        }
    }
} 