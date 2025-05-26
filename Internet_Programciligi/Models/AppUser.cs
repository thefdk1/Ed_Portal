using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internet_Programciligi.Models
{
    [Table("AspNetUsers")]
    public class AppUser : IdentityUser
    {
        [Column("FirstName")]
        public string? FirstName { get; set; }

        [Column("LastName")]
        public string? LastName { get; set; }
        
        // İlişkiler
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Progress> ProgressRecords { get; set; }
        public ICollection<TeacherProfile> TeacherProfiles { get; set; }
    }
} 