using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TWINKLE_TO_DO.Models
{
    public class TaskItem

    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime TaskDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Created";

        public User? User { get; set; }
    }
}


