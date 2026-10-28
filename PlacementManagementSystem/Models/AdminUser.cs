using System.ComponentModel.DataAnnotations;

namespace PlacementManagementSystem.Models
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "Placement Officer";

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = "admin@college.com";

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Admin";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
