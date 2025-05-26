using AutoMapper;
using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;

        public EnrollmentController(
            EnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _enrollmentRepository.GetAll()
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => e.IsActive)
                .ToListAsync();
                
            var enrollmentDTOs = _mapper.Map<List<EnrollmentDTO>>(enrollments);
            return Ok(enrollmentDTOs);
        }

        [Authorize]
        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(userId);
            var enrollmentDTOs = _mapper.Map<List<EnrollmentDTO>>(enrollments);

            // Resim URL'lerini düzenle
            foreach (var dto in enrollmentDTOs)
            {
                if (!string.IsNullOrEmpty(dto.CourseImageUrl))
                {
                    if (!dto.CourseImageUrl.StartsWith("http"))
                    {
                        // Request.Scheme: http/https
                        // Request.Host: domain
                        var baseUrl = $"{Request.Scheme}://{Request.Host}";
                        dto.CourseImageUrl = baseUrl + dto.CourseImageUrl;
                    }
                }
            }

            return Ok(enrollmentDTOs);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrollment = await _enrollmentRepository.GetAll()
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
            
            if (enrollment == null)
            {
                return NotFound();
            }

            // Admin değilse, sadece kendi kaydını görüntüleyebilir
            if (!User.IsInRole("Admin") && enrollment.UserId != userId)
            {
                return Forbid();
            }

            var enrollmentDTO = _mapper.Map<EnrollmentDTO>(enrollment);
            return Ok(enrollmentDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, UpdateEnrollmentDTO updateEnrollmentDTO)
        {
            if (ModelState.IsValid)
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null)
                {
                    return NotFound();
                }

                _mapper.Map(updateEnrollmentDTO, enrollment);
                enrollment.UpdatedDate = DateTime.Now;

                _enrollmentRepository.Update(enrollment);
                await _enrollmentRepository.SaveChangesAsync();

                return Ok(_mapper.Map<EnrollmentDTO>(enrollment));
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrollment = await _enrollmentRepository.GetByIdAsync(id);
            
            if (enrollment == null)
            {
                return NotFound();
            }

            // Admin değilse, sadece kendi kaydını iptal edebilir
            if (!User.IsInRole("Admin") && enrollment.UserId != userId)
            {
                return Forbid();
            }

            // Kaydı silmek yerine pasif yap
            enrollment.IsActive = false;
            enrollment.UpdatedDate = DateTime.Now;
            
            _enrollmentRepository.Update(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return NoContent();
        }
    }
} 