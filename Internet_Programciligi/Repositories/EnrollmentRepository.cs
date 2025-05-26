using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Enrollment>> GetUserEnrollmentsAsync(string userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .Where(e => e.UserId == userId && e.IsActive)
                .ToListAsync();
        }

        public async Task<Enrollment> GetDetailedEnrollmentAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
} 