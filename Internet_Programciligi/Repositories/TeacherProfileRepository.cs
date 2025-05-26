using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class TeacherProfileRepository : GenericRepository<TeacherProfile>
    {
        private readonly AppDbContext _context;

        public TeacherProfileRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<TeacherProfile> GetByIdAsync(int id)
        {
            return await _context.TeacherProfiles
                .Include(tp => tp.User)
                .Include(tp => tp.Category)
                .Include(tp => tp.Courses)
                .FirstOrDefaultAsync(tp => tp.Id == id);
        }

        public async Task<TeacherProfile> GetTeacherProfileByUserIdAsync(string userId)
        {
            return await _context.TeacherProfiles
                .Include(tp => tp.User)
                .Include(tp => tp.Category)
                .FirstOrDefaultAsync(tp => tp.UserId == userId);
        }

        public async Task<List<TeacherProfile>> GetAllTeachersWithCoursesAsync()
        {
            return await _context.TeacherProfiles
                .Include(tp => tp.User)
                .Include(tp => tp.Category)
                .Include(tp => tp.Courses)
                .ToListAsync();
        }
    }
} 