using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly ProgressRepository _progressRepository;
        private readonly LessonRepository _lessonRepository;

        public ProgressController(
            ProgressRepository progressRepository,
            LessonRepository lessonRepository)
        {
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
        }

        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetByLessonId(int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                return NotFound(new { message = "Ders bulunamadı." });
            }

            var progress = await _progressRepository.GetProgressByLessonAndUserAsync(lessonId, userId);
            
            if (progress == null)
            {
                return NotFound(new { message = "İlerleme kaydı bulunamadı." });
            }

            var progressDto = new ProgressDTO
            {
                Id = progress.Id,
                IsCompleted = progress.IsCompleted,
                WatchedSeconds = progress.WatchedSeconds,
                LastWatchDate = progress.LastWatchDate,
                UserId = progress.UserId,
                LessonId = progress.LessonId,
                LessonTitle = lesson.Title,
                CreatedDate = progress.CreatedDate
            };

            return Ok(progressDto);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var progressList = await _progressRepository.GetAllProgressForUserInCourseAsync(userId, courseId);
            
            var progressDtos = progressList.Select(p => new ProgressDTO
            {
                Id = p.Id,
                IsCompleted = p.IsCompleted,
                WatchedSeconds = p.WatchedSeconds,
                LastWatchDate = p.LastWatchDate,
                UserId = p.UserId,
                LessonId = p.LessonId,
                LessonTitle = p.Lesson?.Title,
                CreatedDate = p.CreatedDate
            }).ToList();

            return Ok(progressDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProgressDTO createProgressDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var lesson = await _lessonRepository.GetByIdAsync(createProgressDto.LessonId);
            if (lesson == null)
            {
                return BadRequest(new { message = "Geçerli bir ders belirtilmelidir." });
            }

            // Mevcut bir ilerleme kaydı var mı kontrol et
            var existingProgress = await _progressRepository.GetProgressByLessonAndUserAsync(createProgressDto.LessonId, userId);
            
            if (existingProgress != null)
            {
                // Mevcut kaydı güncelle
                existingProgress.WatchedSeconds = createProgressDto.WatchedSeconds;
                existingProgress.IsCompleted = createProgressDto.IsCompleted;
                existingProgress.LastWatchDate = DateTime.Now;
                existingProgress.UpdatedDate = DateTime.Now;

                _progressRepository.Update(existingProgress);
                await _progressRepository.SaveChangesAsync();

                return Ok(new ProgressDTO
                {
                    Id = existingProgress.Id,
                    IsCompleted = existingProgress.IsCompleted,
                    WatchedSeconds = existingProgress.WatchedSeconds,
                    LastWatchDate = existingProgress.LastWatchDate,
                    UserId = existingProgress.UserId,
                    LessonId = existingProgress.LessonId,
                    LessonTitle = lesson.Title,
                    CreatedDate = existingProgress.CreatedDate
                });
            }

            // Yeni kayıt oluştur
            var progress = new Progress
            {
                IsCompleted = createProgressDto.IsCompleted,
                WatchedSeconds = createProgressDto.WatchedSeconds,
                LastWatchDate = DateTime.Now,
                UserId = userId,
                LessonId = createProgressDto.LessonId
            };

            await _progressRepository.AddAsync(progress);
            await _progressRepository.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, new ProgressDTO
            {
                Id = progress.Id,
                IsCompleted = progress.IsCompleted,
                WatchedSeconds = progress.WatchedSeconds,
                LastWatchDate = progress.LastWatchDate,
                UserId = progress.UserId,
                LessonId = progress.LessonId,
                LessonTitle = lesson.Title,
                CreatedDate = progress.CreatedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProgressDTO updateProgressDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var progress = await _progressRepository.GetByIdAsync(id);
            
            if (progress == null)
            {
                return NotFound(new { message = "İlerleme kaydı bulunamadı." });
            }

            if (progress.UserId != userId)
            {
                return Forbid();
            }

            progress.IsCompleted = updateProgressDto.IsCompleted;
            progress.WatchedSeconds = updateProgressDto.WatchedSeconds;
            progress.LastWatchDate = DateTime.Now;
            progress.UpdatedDate = DateTime.Now;

            _progressRepository.Update(progress);
            await _progressRepository.SaveChangesAsync();

            var lesson = await _lessonRepository.GetByIdAsync(progress.LessonId);

            return Ok(new ProgressDTO
            {
                Id = progress.Id,
                IsCompleted = progress.IsCompleted,
                WatchedSeconds = progress.WatchedSeconds,
                LastWatchDate = progress.LastWatchDate,
                UserId = progress.UserId,
                LessonId = progress.LessonId,
                LessonTitle = lesson?.Title,
                CreatedDate = progress.CreatedDate
            });
        }
    }
} 