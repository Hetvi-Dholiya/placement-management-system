using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlacementManagementSystem.Models
{
    public class Placement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Student is required.")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Placement drive is required.")]
        [Display(Name = "Placement Drive")]
        public int PlacementDriveId { get; set; }

        [Required(ErrorMessage = "Placement date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Placement Date")]
        public DateTime PlacementDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Salary package is required.")]
        [Range(0.50, 100.00, ErrorMessage = "Package must be between 0.50 and 100.00 LPA.")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Package (LPA)")]
        public decimal Package { get; set; }

        [Required(ErrorMessage = "Job role is required.")]
        [StringLength(100)]
        [Display(Name = "Job Role")]
        public string JobRole { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Placed"; // Placed, Pending, Cancelled

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        [ForeignKey("PlacementDriveId")]
        public virtual PlacementDrive? PlacementDrive { get; set; }
    }
}
