using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly LessonRepository _lessonRepository;
        private readonly CourseRepository _courseRepository;
        private readonly ProgressRepository _progressRepository;

        public LessonController(
            LessonRepository lessonRepository,
            CourseRepository courseRepository,
            ProgressRepository progressRepository)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _progressRepository = progressRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lessons = _lessonRepository.GetAll().ToList();
            
            var lessonDtos = lessons.Select(l => new LessonDTO
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                Duration = l.Duration,
                OrderNumber = l.OrderNumber,
                IsActive = l.IsActive,
                CourseId = l.CourseId,
                CreatedDate = l.CreatedDate
            }).ToList();

            return Ok(lessonDtos);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound(new { message = "Kurs bulunamadı." });
            }

            var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
            
            var lessonDtos = lessons.Select(l => new LessonDTO
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                Duration = l.Duration,
                OrderNumber = l.OrderNumber,
                IsActive = l.IsActive,
                CourseId = l.CourseId,
                CourseName = course.Name,
                CreatedDate = l.CreatedDate
            }).ToList();

            return Ok(lessonDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string userId = null)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            
            if (lesson == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
            
            var lessonDto = new LessonDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsActive = lesson.IsActive,
                CourseId = lesson.CourseId,
                CourseName = course?.Name,
                CreatedDate = lesson.CreatedDate
            };
            
            // Kullanıcı ID'si verilmişse ilerleme bilgisini de ekle
            if (!string.IsNullOrEmpty(userId))
            {
                var progress = await _progressRepository.GetProgressByLessonAndUserAsync(id, userId);
                if (progress != null)
                {
                    // İlerleme bilgisi burada DTO'ya eklenebilir
                }
            }

            return Ok(lessonDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonDTO createLessonDto)
        {
            var course = await _courseRepository.GetByIdAsync(createLessonDto.CourseId);
            if (course == null)
            {
                return BadRequest(new { message = "Geçerli bir kurs belirtilmelidir." });
            }

            var lesson = new Lesson
            {
                Title = createLessonDto.Title,
                Description = createLessonDto.Description,
                VideoUrl = createLessonDto.VideoUrl,
                Duration = createLessonDto.Duration,
                OrderNumber = createLessonDto.OrderNumber,
                CourseId = createLessonDto.CourseId,
                IsActive = true
            };

            await _lessonRepository.AddAsync(lesson);
            await _lessonRepository.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, new LessonDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsActive = lesson.IsActive,
                CourseId = lesson.CourseId,
                CourseName = course.Name,
                CreatedDate = lesson.CreatedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLessonDTO updateLessonDto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            
            if (lesson == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            lesson.Title = updateLessonDto.Title;
            lesson.Description = updateLessonDto.Description;
            lesson.VideoUrl = updateLessonDto.VideoUrl;
            lesson.Duration = updateLessonDto.Duration;
            lesson.OrderNumber = updateLessonDto.OrderNumber;
            lesson.IsActive = updateLessonDto.IsActive;
            lesson.UpdatedDate = DateTime.Now;

            _lessonRepository.Update(lesson);
            await _lessonRepository.SaveChangesAsync();

            var course = await _courseRepository.GetByIdAsync(lesson.CourseId);

            return Ok(new LessonDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Duration = lesson.Duration,
                OrderNumber = lesson.OrderNumber,
                IsActive = lesson.IsActive,
                CourseId = lesson.CourseId,
                CourseName = course?.Name,
                CreatedDate = lesson.CreatedDate
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            
            if (lesson == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            _lessonRepository.Remove(lesson);
            await _lessonRepository.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteLesson(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            // Mevcut ilerleme kaydını kontrol et
            var progress = await _progressRepository.GetProgressByLessonAndUserAsync(id, userId);
            
            if (progress != null)
            {
                // Mevcut kaydı güncelle
                progress.IsCompleted = true;
                progress.LastWatchDate = DateTime.Now;
                progress.UpdatedDate = DateTime.Now;

                _progressRepository.Update(progress);
            }
            else
            {
                // Yeni ilerleme kaydı oluştur
                progress = new Progress
                {
                    IsCompleted = true,
                    WatchedSeconds = 0, // İlk aşamada sadece tamamlandı olarak işaretliyoruz
                    LastWatchDate = DateTime.Now,
                    UserId = userId,
                    LessonId = id
                };

                await _progressRepository.AddAsync(progress);
            }

            await _progressRepository.SaveChangesAsync();

            return Ok(new { message = "Ders başarıyla tamamlandı olarak işaretlendi." });
        }
    }
} 