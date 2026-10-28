using System.ComponentModel.DataAnnotations;

namespace PlacementManagementSystem.ViewModels
{
    public class PlacementCreateViewModel
    {
        public int? ApplicationId { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please select a placement drive.")]
        [Display(Name = "Placement Drive")]
        public int PlacementDriveId { get; set; }

        [Required(ErrorMessage = "Placement date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Placement Date")]
        public DateTime PlacementDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Salary Package is required.")]
        [Range(0.50, 100.00, ErrorMessage = "Package must be between 0.50 and 100.00 LPA.")]
        [Display(Name = "Package (LPA)")]
        public decimal Package { get; set; }

        [Required(ErrorMessage = "Job role is required.")]
        [StringLength(100)]
        [Display(Name = "Job Role")]
        public string JobRole { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Placement Status")]
        public string Status { get; set; } = "Placed"; // Placed, Pending, Cancelled
    }

    public class ApplicationStatusViewModel
    {
        public int ApplicationId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string EnrollmentNo { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a status.")]
        public string Status { get; set; } = "Applied";

        [StringLength(500)]
        [Display(Name = "Remarks / Feedback")]
        public string? Remarks { get; set; }
    }
}
