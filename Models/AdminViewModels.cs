using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TWINKLE_TO_DO.Models
{
    public class AdminDashboardViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

    public class AdminEditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }

    public class AdminCreateTaskViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime TaskDate { get; set; } = DateTime.Now.AddDays(1);

        public List<User>? Users { get; set; }
    }

    public class AdminEditTaskViewModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime TaskDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        public List<User>? Users { get; set; }
    }
}