namespace Internet_Programciligi.Models
{
    public class Enrollment : BaseEntity
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
        
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
} 