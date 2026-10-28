using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlacementManagementSystem.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Placement Drive")]
        public int PlacementDriveId { get; set; }

        [Display(Name = "Application Date")]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Applied"; // Applied, Shortlisted, Interview, Selected, Rejected

        [StringLength(500)]
        public string? Remarks { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("PlacementDriveId")]
        public virtual PlacementDrive? PlacementDrive { get; set; }
    }
}
