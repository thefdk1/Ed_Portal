namespace Internet_Programciligi.Models
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; } // Video süresi (dakika cinsinden)
        public int OrderNumber { get; set; } // Ders sırası
        public bool IsActive { get; set; } = true;
        
        public int CourseId { get; set; }
        public Course Course { get; set; }
        
        public ICollection<Progress> ProgressRecords { get; set; }
    }
} 