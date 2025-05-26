using System.ComponentModel.DataAnnotations.Schema;

namespace Internet_Programciligi.Models
{
    [Table("TeacherProfiles")]
    public class TeacherProfile : BaseEntity
    {
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        
        [Column("Name")]
        public string Name { get; set; }
        
        [Column("ProfilePictureUrl")]
        public string ProfilePictureUrl { get; set; }

        [Column("Biography")]
        public string Biography { get; set; }
        
        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        
        public ICollection<Course> Courses { get; set; }
    }
} 