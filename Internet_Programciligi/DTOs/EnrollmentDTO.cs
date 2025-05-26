namespace Internet_Programciligi.DTOs
{
    public class EnrollmentDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseImageUrl { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateEnrollmentDTO
    {
        public int CourseId { get; set; }
    }

    public class UpdateEnrollmentDTO
    {
        public bool IsActive { get; set; }
    }
} 