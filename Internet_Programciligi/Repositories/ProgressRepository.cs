using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class ProgressRepository : GenericRepository<Progress>
    {
        private readonly AppDbContext _context;

        public ProgressRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Progress> GetProgressByLessonAndUserAsync(int lessonId, string userId)
        {
            return await _context.ProgressRecords
                .FirstOrDefaultAsync(p => p.LessonId == lessonId && p.UserId == userId);
        }

        public async Task<List<Progress>> GetAllProgressForUserInCourseAsync(string userId, int courseId)
        {
            return await _context.ProgressRecords
                .Include(p => p.Lesson)
                .Where(p => p.UserId == userId && p.Lesson.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<int> GetCompletedLessonsCountForCourseAsync(string userId, int courseId)
        {
            return await _context.ProgressRecords
                .Include(p => p.Lesson)
                .Where(p => p.UserId == userId && 
                       p.Lesson.CourseId == courseId && 
                       p.IsCompleted)
                .CountAsync();
        }
    }
} 