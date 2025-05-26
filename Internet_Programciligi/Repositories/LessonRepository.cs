using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>
    {
        private readonly AppDbContext _context;

        public LessonRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.OrderNumber)
                .ToListAsync();
        }

        public async Task<Lesson> GetLessonWithProgressAsync(int lessonId, string userId)
        {
            return await _context.Lessons
                .Include(l => l.ProgressRecords.Where(p => p.UserId == userId))
                .FirstOrDefaultAsync(l => l.Id == lessonId);
        }
    }
} 