using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWINKLE_TO_DO.Models
{
    public class UserSettings
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Theme { get; set; } = "system";

        public bool NotificationsEnabled { get; set; } = true;

        public User? User { get; set; }
    }
}
