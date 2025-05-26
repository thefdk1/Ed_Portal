using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internet_Programciligi.Models
{
    public abstract class BaseEntity
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }
} 