using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class CourseRepository : GenericRepository<Course>
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllWithEnrollmentsAsync()
        {
            var courses = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Category)
                .Include(c => c.TeacherProfile)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews)
                .AsNoTracking()  // Performans için
                .ToListAsync();

            // Debug için kayıt sayılarını kontrol et
            foreach (var course in courses)
            {
                var activeEnrollments = course.Enrollments?.Count(e => e.IsActive) ?? 0;
                System.Diagnostics.Debug.WriteLine($"Kurs: {course.Name}, Aktif Kayıt Sayısı: {activeEnrollments}");
            }

            return courses;
        }

        public async Task<Course> GetWithEnrollmentsAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.TeacherProfile)
                .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
} 