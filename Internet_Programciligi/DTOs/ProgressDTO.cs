namespace Internet_Programciligi.DTOs
{
    public class ProgressDTO
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
        public int WatchedSeconds { get; set; }
        public DateTime LastWatchDate { get; set; }
        public string UserId { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateProgressDTO
    {
        public bool IsCompleted { get; set; }
        public int WatchedSeconds { get; set; }
        public int LessonId { get; set; }
    }

    public class UpdateProgressDTO
    {
        public bool IsCompleted { get; set; }
        public int WatchedSeconds { get; set; }
    }
} 