using PlacementManagementSystem.Models;

namespace PlacementManagementSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalCompanies { get; set; }
        public int ActivePlacementDrives { get; set; }
        public int TotalApplications { get; set; }
        public int TotalSelectedApplications { get; set; }
        public int TotalPlacedStudents { get; set; }
        public decimal AveragePackage { get; set; }
        public decimal HighestPackage { get; set; }

        public List<Application> RecentApplications { get; set; } = new List<Application>();
        public List<PlacementDrive> UpcomingPlacementDrives { get; set; } = new List<PlacementDrive>();
        public List<Placement> RecentlyPlacedStudents { get; set; } = new List<Placement>();

        // Statistics for charts/visualizations
        public Dictionary<string, int> ApplicationsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PlacementsByDepartment { get; set; } = new Dictionary<string, int>();
    }

    public class StudentDashboardViewModel
    {
        public Student Student { get; set; } = null!;
        public int AvailableDrivesCount { get; set; }
        public int AppliedDrivesCount { get; set; }
        public int ShortlistedCount { get; set; }
        public int SelectedCount { get; set; }
        public bool IsPlaced { get; set; }
        public Placement? FinalPlacement { get; set; }

        public List<PlacementDriveCardViewModel> RecentEligibleDrives { get; set; } = new List<PlacementDriveCardViewModel>();
        public List<Application> MyRecentApplications { get; set; } = new List<Application>();
    }

    public class PlacementDriveCardViewModel
    {
        public PlacementDrive Drive { get; set; } = null!;
        public bool IsEligible { get; set; }
        public bool HasApplied { get; set; }
        public bool IsDeadlinePassed { get; set; }
        public string? IneligibilityReason { get; set; }
        public string? ApplicationStatus { get; set; }
    }
}
