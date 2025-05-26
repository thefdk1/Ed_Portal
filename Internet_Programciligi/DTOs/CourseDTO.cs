namespace Internet_Programciligi.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YoutubeLink { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ImageUrl { get; set; }
        
        // Yeni alanlar
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? TeacherProfileId { get; set; }
        public string TeacherName { get; set; }
        public int LessonsCount { get; set; }
        public double AverageRating { get; set; }
        public int StudentCount { get; set; }
    }

    public class CreateCourseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YoutubeLink { get; set; }
        public int? CategoryId { get; set; }
        public int? TeacherProfileId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateCourseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? YoutubeLink { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }
        public int? TeacherProfileId { get; set; }
        public string? ImageUrl { get; set; }
    }
} 