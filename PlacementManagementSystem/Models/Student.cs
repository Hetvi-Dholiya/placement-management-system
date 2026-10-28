using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlacementManagementSystem.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enrollment number is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Enrollment number must be between 3 and 20 characters.")]
        [Display(Name = "Enrollment No")]
        public string EnrollmentNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10)]
        public string Gender { get; set; } = "Male";

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8.")]
        public int Semester { get; set; } = 7;

        [Required(ErrorMessage = "CGPA is required.")]
        [Range(0.00, 10.00, ErrorMessage = "CGPA must be between 0.00 and 10.00.")]
        [Column(TypeName = "decimal(4,2)")]
        public decimal CGPA { get; set; }

        [Display(Name = "Technical Skills")]
        [StringLength(255)]
        public string Skills { get; set; } = string.Empty;

        [Display(Name = "Resume File")]
        [StringLength(255)]
        public string? ResumeFileName { get; set; }

        [Display(Name = "Profile Picture")]
        [StringLength(255)]
        public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "Student";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public virtual ICollection<Placement> Placements { get; set; } = new List<Placement>();
    }
}
