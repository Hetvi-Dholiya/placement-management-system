using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Models;

namespace PlacementManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<PlacementDrive> PlacementDrives { get; set; } = null!;
        public DbSet<Application> Applications { get; set; } = null!;
        public DbSet<Placement> Placements { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student Constraints
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.EnrollmentNo)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // Prevent duplicate applications: One student can apply to a drive at most once
            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.StudentId, a.PlacementDriveId })
                .IsUnique();

            // Configure Relationships
            // Company -> PlacementDrives (1 to Many)
            modelBuilder.Entity<Company>()
                .HasMany(c => c.PlacementDrives)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlacementDrive -> Applications (1 to Many)
            modelBuilder.Entity<PlacementDrive>()
                .HasMany(p => p.Applications)
                .WithOne(a => a.PlacementDrive)
                .HasForeignKey(a => a.PlacementDriveId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -> Applications (1 to Many)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Placements Relationships
            modelBuilder.Entity<Placement>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Placements)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Placement>()
                .HasOne(p => p.Company)
                .WithMany(c => c.Placements)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Placement>()
                .HasOne(p => p.PlacementDrive)
                .WithMany(pd => pd.Placements)
                .HasForeignKey(p => p.PlacementDriveId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precision for decimals
            modelBuilder.Entity<Student>()
                .Property(s => s.CGPA)
                .HasPrecision(4, 2);

            modelBuilder.Entity<PlacementDrive>()
                .Property(p => p.EligibilityCGPA)
                .HasPrecision(4, 2);

            modelBuilder.Entity<PlacementDrive>()
                .Property(p => p.SalaryPackage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Placement>()
                .Property(p => p.Package)
                .HasPrecision(5, 2);
        }
    }
}
