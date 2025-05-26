using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeacherProfileController : ControllerBase
    {
        private readonly TeacherProfileRepository _teacherProfileRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<TeacherProfileController> _logger;

        public TeacherProfileController(
            TeacherProfileRepository teacherProfileRepository,
            UserManager<AppUser> userManager,
            IWebHostEnvironment hostingEnvironment,
            ILogger<TeacherProfileController> logger)
        {
            _teacherProfileRepository = teacherProfileRepository;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _teacherProfileRepository.GetAllTeachersWithCoursesAsync();
            
            var teacherDtos = teachers.Select(t => new TeacherProfileDTO
            {
                Id = t.Id,
                UserId = t.UserId,
                Name = t.Name,
                FullName = t.Name,
                ProfilePictureUrl = t.ProfilePictureUrl,
                Biography = t.Biography,
                CategoryId = t.CategoryId,
                Category = t.Category != null ? new CategoryDTO 
                { 
                    Id = t.Category.Id, 
                    Name = t.Category.Name 
                } : null,
                Courses = t.Courses?.Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedDate = c.CreatedDate
                }).ToList(),
                CreatedDate = t.CreatedDate
            }).ToList();

            return Ok(teacherDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _teacherProfileRepository.GetByIdAsync(id);
            
            if (teacher == null)
            {
                return NotFound(new { message = "Eğitmen profili bulunamadı." });
            }

            var user = await _userManager.FindByIdAsync(teacher.UserId);
            
            var teacherDto = new TeacherProfileDTO
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                Name = teacher.Name,
                FullName = teacher.Name,
                ProfilePictureUrl = teacher.ProfilePictureUrl,
                Biography = teacher.Biography,
                CategoryId = teacher.CategoryId,
                Category = teacher.Category != null ? new CategoryDTO 
                { 
                    Id = teacher.Category.Id, 
                    Name = teacher.Category.Name 
                } : null,
                CreatedDate = teacher.CreatedDate
            };

            return Ok(teacherDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var teacher = await _teacherProfileRepository.GetTeacherProfileByUserIdAsync(userId);
            
            if (teacher == null)
            {
                return NotFound(new { message = "Eğitmen profili bulunamadı." });
            }

            var user = await _userManager.FindByIdAsync(teacher.UserId);
            
            var teacherDto = new TeacherProfileDTO
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = $"{user?.FirstName} {user?.LastName}",
                ProfilePictureUrl = teacher.ProfilePictureUrl,
                Biography = teacher.Biography,
                CreatedDate = teacher.CreatedDate
            };

            return Ok(teacherDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeacherProfileDTO createTeacherProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "Kullanıcı bulunamadı." });
            }

            // Kullanıcının adını güncelle
            if (!string.IsNullOrEmpty(createTeacherProfileDto.Name))
            {
                var nameParts = createTeacherProfileDto.Name.Split(' ', 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                await _userManager.UpdateAsync(user);
            }

            var teacherProfile = new TeacherProfile
            {
                UserId = userId,
                Name = createTeacherProfileDto.Name,
                ProfilePictureUrl = createTeacherProfileDto.ProfilePictureUrl,
                Biography = createTeacherProfileDto.Biography,
                CategoryId = createTeacherProfileDto.CategoryId
            };

            await _teacherProfileRepository.AddAsync(teacherProfile);
            await _teacherProfileRepository.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, new TeacherProfileDTO
            {
                Id = teacherProfile.Id,
                UserId = teacherProfile.UserId,
                FullName = $"{user.FirstName} {user.LastName}",
                ProfilePictureUrl = teacherProfile.ProfilePictureUrl,
                Biography = teacherProfile.Biography,
                CategoryId = teacherProfile.CategoryId,
                CreatedDate = teacherProfile.CreatedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTeacherProfileDTO updateTeacherProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var teacherProfile = await _teacherProfileRepository.GetByIdAsync(id);
            
            if (teacherProfile == null)
            {
                return NotFound(new { message = "Eğitmen profili bulunamadı." });
            }

            if (teacherProfile.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            teacherProfile.Name = updateTeacherProfileDto.Name;
            teacherProfile.ProfilePictureUrl = updateTeacherProfileDto.ProfilePictureUrl;
            teacherProfile.Biography = updateTeacherProfileDto.Biography;
            teacherProfile.CategoryId = updateTeacherProfileDto.CategoryId;
            teacherProfile.UpdatedDate = DateTime.Now;

            _teacherProfileRepository.Update(teacherProfile);
            await _teacherProfileRepository.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(teacherProfile.UserId);

            return Ok(new TeacherProfileDTO
            {
                Id = teacherProfile.Id,
                UserId = teacherProfile.UserId,
                FullName = $"{user?.FirstName} {user?.LastName}",
                ProfilePictureUrl = teacherProfile.ProfilePictureUrl,
                Biography = teacherProfile.Biography,
                CategoryId = teacherProfile.CategoryId,
                CreatedDate = teacherProfile.CreatedDate
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var teacherProfile = await _teacherProfileRepository.GetByIdAsync(id);
            
            if (teacherProfile == null)
            {
                return NotFound(new { message = "Eğitmen profili bulunamadı." });
            }

            if (teacherProfile.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _teacherProfileRepository.Remove(teacherProfile);
            await _teacherProfileRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] int? teacherId = null)
        {
            try
            {
                _logger.LogInformation($"Resim yükleme başladı - TeacherId: {teacherId}");

                if (image == null || image.Length == 0)
                {
                    _logger.LogWarning("Resim dosyası boş");
                    return BadRequest(new { message = "Resim dosyası bulunamadı." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"Kullanıcı ID: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Kullanıcı girişi yapılmamış");
                    return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
                }

                TeacherProfile teacherProfile = null;
                string filePrefix = "temp"; // Varsayılan prefix

                if (teacherId.HasValue)
                {
                    _logger.LogInformation($"Mevcut öğretmen için resim yükleniyor. TeacherId: {teacherId}");
                    teacherProfile = await _teacherProfileRepository.GetByIdAsync(teacherId.Value);
                    
                    if (teacherProfile == null)
                    {
                        _logger.LogWarning($"Öğretmen bulunamadı. TeacherId: {teacherId}");
                        return NotFound(new { message = "Öğretmen profili bulunamadı." });
                    }

                    _logger.LogInformation($"Bulunan öğretmen - ID: {teacherProfile.Id}, UserId: {teacherProfile.UserId}");

                    if (teacherProfile.UserId != userId && !User.IsInRole("Admin"))
                    {
                        _logger.LogWarning($"Yetkisiz erişim. UserId: {userId}, TeacherUserId: {teacherProfile.UserId}");
                        return Forbid();
                    }

                    filePrefix = $"teacher_{teacherProfile.Id}";
                }
                else
                {
                    _logger.LogInformation("Yeni öğretmen için geçici resim yükleniyor");
                    filePrefix = $"new_teacher_{DateTime.Now.Ticks}";
                }

                // Resim dosyası uzantısını kontrol et
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                _logger.LogInformation($"Resim uzantısı: {extension}");

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                {
                    _logger.LogWarning($"Geçersiz dosya formatı: {extension}");
                    return BadRequest(new { message = "Geçersiz dosya formatı. JPG, PNG veya GIF kullanmalısınız." });
                }

                // Dosya boyutu kontrolü (5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning($"Dosya boyutu çok büyük: {image.Length} bytes");
                    return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır." });
                }

                try
                {
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    _logger.LogInformation($"WebRoot Path: {webRootPath}");

                    if (string.IsNullOrEmpty(webRootPath))
                    {
                        webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        _logger.LogInformation($"Yeni WebRoot Path oluşturuldu: {webRootPath}");
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                    }

                    var uploadsFolder = Path.Combine(webRootPath, "uploads", "teachers");
                    _logger.LogInformation($"Uploads klasörü: {uploadsFolder}");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        _logger.LogInformation("Uploads klasörü oluşturuldu");
                    }

                    // Benzersiz dosya adı oluştur
                    var uniqueFileName = $"{filePrefix}_{DateTime.Now.Ticks}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    _logger.LogInformation($"Yeni dosya yolu: {filePath}");

                    // Eğer mevcut öğretmen ise eski resmi sil
                    if (teacherProfile != null && !string.IsNullOrEmpty(teacherProfile.ProfilePictureUrl))
                    {
                        var oldFilePath = Path.Combine(webRootPath, teacherProfile.ProfilePictureUrl.TrimStart('/'));
                        _logger.LogInformation($"Eski dosya yolu: {oldFilePath}");

                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                                _logger.LogInformation("Eski dosya silindi");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"Eski resim silinirken hata oluştu: {ex.Message}");
                            }
                        }
                    }

                    // Yeni resmi kaydet
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    _logger.LogInformation("Yeni dosya kaydedildi");

                    var imageUrl = $"/uploads/teachers/{uniqueFileName}";

                    // Eğer mevcut öğretmen ise veritabanını güncelle
                    if (teacherProfile != null)
                    {
                        teacherProfile.ProfilePictureUrl = imageUrl;
                        teacherProfile.UpdatedDate = DateTime.Now;
                        
                        _teacherProfileRepository.Update(teacherProfile);
                        await _teacherProfileRepository.SaveChangesAsync();
                        _logger.LogInformation($"Veritabanı güncellendi. Yeni URL: {imageUrl}");
                    }

                    return Ok(new { imageUrl = imageUrl });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Dosya sistemi hatası: {ex.Message}");
                    _logger.LogError($"Stack trace: {ex.StackTrace}");
                    return StatusCode(500, new { message = $"Dosya sistemi hatası: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Genel hata: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Resim yüklenirken bir hata oluştu: {ex.Message}" });
            }
        }
    }
} 