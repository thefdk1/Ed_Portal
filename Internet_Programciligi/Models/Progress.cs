namespace Internet_Programciligi.Models
{
    public class Progress : BaseEntity
    {
        public bool IsCompleted { get; set; } = false;
        public int WatchedSeconds { get; set; } = 0;
        public DateTime LastWatchDate { get; set; } = DateTime.Now;
        
        public string UserId { get; set; }
        public AppUser User { get; set; }
        
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
} 