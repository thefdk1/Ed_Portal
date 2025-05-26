using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Internet_Programciligi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly CourseRepository _courseRepository;
        private readonly UserManager<AppUser> _userManager;

        public ReviewController(
            ReviewRepository reviewRepository,
            CourseRepository courseRepository,
            UserManager<AppUser> userManager)
        {
            _reviewRepository = reviewRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = _reviewRepository.GetAll()
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
            
            var reviewDtos = new List<ReviewDTO>();
            
            foreach (var review in reviews)
            {
                var user = await _userManager.FindByIdAsync(review.UserId);
                reviewDtos.Add(new ReviewDTO
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    Rating = review.Rating,
                    UserId = review.UserId,
                    UserName = user?.UserName,
                    CourseId = review.CourseId,
                    CreatedDate = review.CreatedDate
                });
            }

            return Ok(reviewDtos);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound(new { message = "Kurs bulunamadı." });
            }

            var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);
            
            var reviewDtos = reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                Comment = r.Comment,
                Rating = r.Rating,
                UserId = r.UserId,
                UserName = r.User?.UserName,
                CourseId = r.CourseId,
                CreatedDate = r.CreatedDate
            }).ToList();

            return Ok(reviewDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            
            if (review == null)
            {
                return NotFound(new { message = "Yorum bulunamadı." });
            }

            var user = await _userManager.FindByIdAsync(review.UserId);
            
            var reviewDto = new ReviewDTO
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                UserId = review.UserId,
                UserName = user?.UserName,
                CourseId = review.CourseId,
                CreatedDate = review.CreatedDate
            };

            return Ok(reviewDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewDTO createReviewDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var course = await _courseRepository.GetByIdAsync(createReviewDto.CourseId);
            if (course == null)
            {
                return BadRequest(new { message = "Geçerli bir kurs belirtilmelidir." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "Kullanıcı bulunamadı." });
            }

            var review = new Review
            {
                Comment = createReviewDto.Comment,
                Rating = createReviewDto.Rating,
                UserId = userId,
                CourseId = createReviewDto.CourseId
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, new ReviewDTO
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                UserId = review.UserId,
                UserName = user.UserName,
                CourseId = review.CourseId,
                CreatedDate = review.CreatedDate
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateReviewDTO updateReviewDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bu işlem için giriş yapmalısınız." });
            }

            var review = await _reviewRepository.GetByIdAsync(id);
            
            if (review == null)
            {
                return NotFound(new { message = "Yorum bulunamadı." });
            }

            if (review.UserId != userId)
            {
                return Forbid();
            }

            review.Comment = updateReviewDto.Comment;
            review.Rating = updateReviewDto.Rating;
            review.UpdatedDate = DateTime.Now;

            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(review.UserId);

            return Ok(new ReviewDTO
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                UserId = review.UserId,
                UserName = user?.UserName,
                CourseId = review.CourseId,
                CreatedDate = review.CreatedDate
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

            var review = await _reviewRepository.GetByIdAsync(id);
            
            if (review == null)
            {
                return NotFound(new { message = "Yorum bulunamadı." });
            }

            if (review.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveChangesAsync();

            return NoContent();
        }
    }
} 