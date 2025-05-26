namespace Internet_Programciligi.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        public ICollection<Course> Courses { get; set; }
    }
} 