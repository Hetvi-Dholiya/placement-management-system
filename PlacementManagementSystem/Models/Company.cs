using System.ComponentModel.DataAnnotations;

namespace PlacementManagementSystem.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company description is required.")]
        [StringLength(1000)]
        [Display(Name = "Description")]
        public string CompanyDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Industry is required.")]
        [StringLength(50)]
        public string Industry { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(150)]
        public string? Website { get; set; }

        [Required(ErrorMessage = "HR contact name is required.")]
        [StringLength(100)]
        [Display(Name = "HR Name")]
        public string HRName { get; set; } = string.Empty;

        [Required(ErrorMessage = "HR email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        [Display(Name = "HR Email")]
        public string HREmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Please enter a valid contact number.")]
        [Display(Name = "Contact Number")]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<PlacementDrive> PlacementDrives { get; set; } = new List<PlacementDrive>();
        public virtual ICollection<Placement> Placements { get; set; } = new List<Placement>();
    }
}
