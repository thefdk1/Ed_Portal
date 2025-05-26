using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;

namespace Internet_Programciligi.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesWithCoursesAsync()
        {
            return await _context.Categories.Include(c => c.Courses).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdWithCoursesAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
} 