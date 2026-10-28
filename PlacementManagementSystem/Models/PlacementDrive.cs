using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlacementManagementSystem.Models
{
    public class PlacementDrive
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required.")]
        [StringLength(2000)]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minimum CGPA is required.")]
        [Range(0.00, 10.00, ErrorMessage = "CGPA must be between 0.00 and 10.00.")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Eligibility CGPA")]
        public decimal EligibilityCGPA { get; set; }

        [Required(ErrorMessage = "Eligible department is required.")]
        [StringLength(100)]
        [Display(Name = "Eligible Department")]
        public string EligibleDepartment { get; set; } = "All";

        [Required(ErrorMessage = "Required skills are required.")]
        [StringLength(255)]
        [Display(Name = "Required Skills")]
        public string RequiredSkills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary package is required.")]
        [Range(0.50, 100.00, ErrorMessage = "Salary package must be between 0.50 and 100 LPA.")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Salary Package (LPA)")]
        public decimal SalaryPackage { get; set; }

        [Required(ErrorMessage = "Job location is required.")]
        [StringLength(100)]
        [Display(Name = "Job Location")]
        public string JobLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Drive date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Drive Date")]
        public DateTime DriveDate { get; set; }

        [Required(ErrorMessage = "Application deadline is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Application Deadline")]
        public DateTime ApplicationDeadline { get; set; }

        [Required(ErrorMessage = "Number of vacancies is required.")]
        [Range(1, 1000, ErrorMessage = "Number of vacancies must be greater than 0.")]
        [Display(Name = "Number of Vacancies")]
        public int NumberOfVacancies { get; set; } = 1;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Closed, Completed, Upcoming

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public virtual ICollection<Placement> Placements { get; set; } = new List<Placement>();
    }
}
