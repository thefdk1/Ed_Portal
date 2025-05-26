namespace Internet_Programciligi.Models
{
    public class Review : BaseEntity
    {
        public string Comment { get; set; }
        public int Rating { get; set; } // 1-5 arası puan
        
        public string UserId { get; set; }
        public AppUser User { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
} 