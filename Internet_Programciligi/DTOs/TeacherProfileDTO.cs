namespace Internet_Programciligi.DTOs
{
    public class TeacherProfileDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Biography { get; set; }
        public int? CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateTeacherProfileDTO
    {
        public string Name { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Biography { get; set; }
        public int? CategoryId { get; set; }
    }

    public class UpdateTeacherProfileDTO
    {
        public string Name { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Biography { get; set; }
        public int? CategoryId { get; set; }
    }
} 