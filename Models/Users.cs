using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TWINKLE_TO_DO.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public ICollection<TaskItem>? Tasks { get; set; }
        public UserSettings? UserSettings { get; set; }
    }
}