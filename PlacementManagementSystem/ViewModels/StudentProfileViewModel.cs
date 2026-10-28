using System.ComponentModel.DataAnnotations;

namespace PlacementManagementSystem.ViewModels
{
    public class StudentProfileViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Enrollment Number")]
        public string EnrollmentNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone must be 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = "Male";

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Range(1, 8)]
        public int Semester { get; set; }

        [Required]
        [Range(0.00, 10.00)]
        public decimal CGPA { get; set; }

        [Display(Name = "Technical Skills")]
        public string Skills { get; set; } = string.Empty;

        [Display(Name = "Upload New Resume (PDF / DOCX)")]
        public IFormFile? ResumeFile { get; set; }

        public string? ExistingResumeFileName { get; set; }

        [Display(Name = "Profile Picture (JPG, PNG, WEBP)")]
        public IFormFile? ProfilePictureFile { get; set; }

        public string? ExistingProfilePicture { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password (leave empty to keep current)")]
        public string? NewPassword { get; set; }
    }
}
