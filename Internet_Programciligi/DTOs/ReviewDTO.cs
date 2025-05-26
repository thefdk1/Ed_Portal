namespace Internet_Programciligi.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateReviewDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public int CourseId { get; set; }
    }

    public class UpdateReviewDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
} 