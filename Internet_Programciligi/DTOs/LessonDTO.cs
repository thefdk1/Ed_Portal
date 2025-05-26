namespace Internet_Programciligi.DTOs
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int OrderNumber { get; set; }
        public bool IsActive { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateLessonDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int OrderNumber { get; set; }
        public int CourseId { get; set; }
    }

    public class UpdateLessonDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; }
        public int OrderNumber { get; set; }
        public bool IsActive { get; set; }
    }
} 