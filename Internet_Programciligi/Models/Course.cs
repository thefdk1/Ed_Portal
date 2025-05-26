namespace Internet_Programciligi.Models
{
    public class Course : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YoutubeLink { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        
        // Kategori ilişkisi
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        
        // Eğitmen ilişkisi
        public int? TeacherProfileId { get; set; }
        public TeacherProfile TeacherProfile { get; set; }
        
        // İlişkiler
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
} 