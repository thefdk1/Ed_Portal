using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class ReviewRepository : GenericRepository<Review>
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsByCourseIdAsync(int courseId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingForCourseAsync(int courseId)
        {
            if (!await _context.Reviews.AnyAsync(r => r.CourseId == courseId))
            {
                return 0;
            }
            
            return await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .AverageAsync(r => r.Rating);
        }
    }
} 