using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;

namespace PlacementManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Admin
            if (!await context.AdminUsers.AnyAsync())
            {
                var admin = new AdminUser
                {
                    FullName = "College Placement Director",
                    Email = "admin@college.com",
                    Password = PasswordHelper.HashPassword("Admin@123"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                };
                context.AdminUsers.Add(admin);
                await context.SaveChangesAsync();
            }

            // 2. Seed Companies
            if (!await context.Companies.AnyAsync())
            {
                var companies = new List<Company>
                {
                    new Company
                    {
                        CompanyName = "Tata Consultancy Services (TCS)",
                        CompanyDescription = "A global leader in IT services, consulting, and business solutions with a vast network of innovation and delivery centers.",
                        Industry = "IT Services & Consulting",
                        Location = "Mumbai, Maharashtra",
                        Website = "https://www.tcs.com",
                        HRName = "Vikram Sharma",
                        HREmail = "vikram.sharma@tcs.com",
                        ContactNumber = "9823456780",
                        CreatedAt = DateTime.Now.AddDays(-60)
                    },
                    new Company
                    {
                        CompanyName = "Infosys Limited",
                        CompanyDescription = "A global leader in next-generation digital services and consulting, enabling clients in more than 50 countries to navigate digital transformation.",
                        Industry = "Software & Cloud Services",
                        Location = "Bengaluru, Karnataka",
                        Website = "https://www.infosys.com",
                        HRName = "Ananya Iyer",
                        HREmail = "ananya.iyer@infosys.com",
                        ContactNumber = "9876543210",
                        CreatedAt = DateTime.Now.AddDays(-55)
                    },
                    new Company
                    {
                        CompanyName = "Microsoft India",
                        CompanyDescription = "Technology corporation producing computer software, consumer electronics, personal computers, and related enterprise cloud services.",
                        Industry = "Product & Technology",
                        Location = "Hyderabad, Telangana",
                        Website = "https://www.microsoft.com",
                        HRName = "Rajesh Gupta",
                        HREmail = "rajesh.gupta@microsoft.com",
                        ContactNumber = "9123456781",
                        CreatedAt = DateTime.Now.AddDays(-50)
                    },
                    new Company
                    {
                        CompanyName = "Cognizant Technology",
                        CompanyDescription = "Leading professional services company transforming clients' business, operating, and technology models for the digital era.",
                        Industry = "IT & Digital Engineering",
                        Location = "Chennai, Tamil Nadu",
                        Website = "https://www.cognizant.com",
                        HRName = "Pooja Hegde",
                        HREmail = "pooja.hegde@cognizant.com",
                        ContactNumber = "9812345672",
                        CreatedAt = DateTime.Now.AddDays(-45)
                    },
                    new Company
                    {
                        CompanyName = "Goldman Sachs",
                        CompanyDescription = "Leading global financial institution providing investment banking, securities, investment management, and consumer banking services.",
                        Industry = "Fintech & Banking",
                        Location = "Bengaluru, Karnataka",
                        Website = "https://www.goldmansachs.com",
                        HRName = "Kunal Verma",
                        HREmail = "kunal.verma@gs.com",
                        ContactNumber = "9845612345",
                        CreatedAt = DateTime.Now.AddDays(-40)
                    },
                    new Company
                    {
                        CompanyName = "Amazon Web Services",
                        CompanyDescription = "Comprehensive, evolving cloud computing platform provided by Amazon that offers a mixture of IaaS, PaaS and packaged software products.",
                        Industry = "Cloud & E-Commerce",
                        Location = "Bengaluru, Karnataka",
                        Website = "https://aws.amazon.com",
                        HRName = "Sneha Patel",
                        HREmail = "sneha.patel@amazon.com",
                        ContactNumber = "9898765432",
                        CreatedAt = DateTime.Now.AddDays(-35)
                    }
                };

                context.Companies.AddRange(companies);
                await context.SaveChangesAsync();
            }

            // 3. Seed Students
            if (!await context.Students.AnyAsync())
            {
                var defaultPasswordHash = PasswordHelper.HashPassword("Student@123");

                var students = new List<Student>
                {
                    new Student
                    {
                        EnrollmentNo = "21BCSE001",
                        FullName = "Aarav Sharma",
                        Email = "aarav.sharma@college.com",
                        Phone = "9876501234",
                        DateOfBirth = new DateTime(2003, 5, 14),
                        Gender = "Male",
                        Department = "Computer Science and Engineering",
                        Semester = 8,
                        CGPA = 8.85m,
                        Skills = "C#, ASP.NET Core, SQL, JavaScript, React",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-50)
                    },
                    new Student
                    {
                        EnrollmentNo = "21BCSE045",
                        FullName = "Diya Patel",
                        Email = "diya.patel@college.com",
                        Phone = "9823405678",
                        DateOfBirth = new DateTime(2003, 9, 21),
                        Gender = "Female",
                        Department = "Computer Science and Engineering",
                        Semester = 8,
                        CGPA = 9.20m,
                        Skills = "Java, Spring Boot, MySQL, Docker, Python",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-48)
                    },
                    new Student
                    {
                        EnrollmentNo = "21BIT012",
                        FullName = "Rohan Verma",
                        Email = "rohan.verma@college.com",
                        Phone = "9712345678",
                        DateOfBirth = new DateTime(2002, 12, 10),
                        Gender = "Male",
                        Department = "Information Technology",
                        Semester = 8,
                        CGPA = 7.65m,
                        Skills = "Python, Django, PostgreSQL, HTML5, CSS3",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-46)
                    },
                    new Student
                    {
                        EnrollmentNo = "21BECE030",
                        FullName = "Meera Nair",
                        Email = "meera.nair@college.com",
                        Phone = "9654321098",
                        DateOfBirth = new DateTime(2003, 3, 18),
                        Gender = "Female",
                        Department = "Electronics and Communication",
                        Semester = 8,
                        CGPA = 8.10m,
                        Skills = "C++, Embedded Systems, IoT, Python",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-42)
                    },
                    new Student
                    {
                        EnrollmentNo = "21BCSE088",
                        FullName = "Kabir Singh",
                        Email = "kabir.singh@college.com",
                        Phone = "9543216789",
                        DateOfBirth = new DateTime(2002, 8, 30),
                        Gender = "Male",
                        Department = "Computer Science and Engineering",
                        Semester = 8,
                        CGPA = 6.80m,
                        Skills = "JavaScript, Node.js, Express, MongoDB",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-38)
                    },
                    new Student
                    {
                        EnrollmentNo = "22BCSE102",
                        FullName = "Ananya Roy",
                        Email = "ananya.roy@college.com",
                        Phone = "9432167890",
                        DateOfBirth = new DateTime(2004, 1, 15),
                        Gender = "Female",
                        Department = "Computer Science and Engineering",
                        Semester = 7,
                        CGPA = 8.45m,
                        Skills = "C#, .NET MVC, Angular, SQL Server",
                        Password = defaultPasswordHash,
                        Role = "Student",
                        CreatedAt = DateTime.Now.AddDays(-30)
                    }
                };

                context.Students.AddRange(students);
                await context.SaveChangesAsync();
            }

            // 4. Seed Placement Drives
            if (!await context.PlacementDrives.AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var tcs = companies.First(c => c.CompanyName.Contains("TCS"));
                var infosys = companies.First(c => c.CompanyName.Contains("Infosys"));
                var msft = companies.First(c => c.CompanyName.Contains("Microsoft"));
                var cognizant = companies.First(c => c.CompanyName.Contains("Cognizant"));
                var gs = companies.First(c => c.CompanyName.Contains("Goldman Sachs"));
                var aws = companies.First(c => c.CompanyName.Contains("Amazon"));

                var drives = new List<PlacementDrive>
                {
                    new PlacementDrive
                    {
                        CompanyId = msft.Id,
                        JobTitle = "Software Development Engineer - I",
                        JobDescription = "Design and build core cloud components for Azure infrastructure. Work with scalable microservices, high-throughput distributed systems, and real-time data pipelines.",
                        EligibilityCGPA = 8.50m,
                        EligibleDepartment = "Computer Science and Engineering",
                        RequiredSkills = "Data Structures, Algorithms, C#/C++/Java, Cloud Architecture",
                        SalaryPackage = 22.50m,
                        JobLocation = "Hyderabad / Bengaluru",
                        DriveDate = DateTime.Today.AddDays(14),
                        ApplicationDeadline = DateTime.Today.AddDays(7),
                        NumberOfVacancies = 12,
                        Status = "Active",
                        CreatedAt = DateTime.Now.AddDays(-10)
                    },
                    new PlacementDrive
                    {
                        CompanyId = gs.Id,
                        JobTitle = "Financial Technology Analyst",
                        JobDescription = "Build mission-critical algorithmic trading platforms, quantitative risk models, and enterprise financial transaction engines using modern tech stacks.",
                        EligibilityCGPA = 8.00m,
                        EligibleDepartment = "All",
                        RequiredSkills = "Java, Python, SQL, Financial Algorithms, Analytical Reasoning",
                        SalaryPackage = 18.00m,
                        JobLocation = "Bengaluru",
                        DriveDate = DateTime.Today.AddDays(20),
                        ApplicationDeadline = DateTime.Today.AddDays(10),
                        NumberOfVacancies = 8,
                        Status = "Active",
                        CreatedAt = DateTime.Now.AddDays(-8)
                    },
                    new PlacementDrive
                    {
                        CompanyId = tcs.Id,
                        JobTitle = "Systems Engineer (Digital)",
                        JobDescription = "Full stack application development, API design, DevOps pipelines, and enterprise software engineering for international clients.",
                        EligibilityCGPA = 6.50m,
                        EligibleDepartment = "All",
                        RequiredSkills = "C#, Java, Python, Web Technologies, Database Concepts",
                        SalaryPackage = 7.20m,
                        JobLocation = "Pan India",
                        DriveDate = DateTime.Today.AddDays(25),
                        ApplicationDeadline = DateTime.Today.AddDays(12),
                        NumberOfVacancies = 45,
                        Status = "Active",
                        CreatedAt = DateTime.Now.AddDays(-6)
                    },
                    new PlacementDrive
                    {
                        CompanyId = infosys.Id,
                        JobTitle = "Specialist Programmer (Power Programmer)",
                        JobDescription = "Core technology development focusing on high-level coding, machine learning, cloud application modernization, and automation platforms.",
                        EligibilityCGPA = 7.50m,
                        EligibleDepartment = "Computer Science and Engineering",
                        RequiredSkills = "Advanced DSA, Competitive Programming, Full Stack Development",
                        SalaryPackage = 9.50m,
                        JobLocation = "Bengaluru / Pune",
                        DriveDate = DateTime.Today.AddDays(30),
                        ApplicationDeadline = DateTime.Today.AddDays(15),
                        NumberOfVacancies = 20,
                        Status = "Active",
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new PlacementDrive
                    {
                        CompanyId = cognizant.Id,
                        JobTitle = "Programmer Analyst Trainee (GenC Next)",
                        JobDescription = "Develop web applications, microservices, cloud deployments, and automated testing suites for digital enterprise solutions.",
                        EligibilityCGPA = 7.00m,
                        EligibleDepartment = "All",
                        RequiredSkills = "OOPs, SQL, HTML, CSS, JavaScript, Problem Solving",
                        SalaryPackage = 6.75m,
                        JobLocation = "Chennai / Hyderabad",
                        DriveDate = DateTime.Today.AddDays(18),
                        ApplicationDeadline = DateTime.Today.AddDays(9),
                        NumberOfVacancies = 35,
                        Status = "Active",
                        CreatedAt = DateTime.Now.AddDays(-4)
                    },
                    new PlacementDrive
                    {
                        CompanyId = aws.Id,
                        JobTitle = "Cloud Support Associate",
                        JobDescription = "Assist enterprise customers in architecting reliable cloud systems on AWS, troubleshooting distributed cloud incidents, and optimizing infrastructure.",
                        EligibilityCGPA = 7.20m,
                        EligibleDepartment = "Information Technology",
                        RequiredSkills = "Networking, Linux, AWS Cloud Services, Python Scripting",
                        SalaryPackage = 11.00m,
                        JobLocation = "Bengaluru",
                        DriveDate = DateTime.Today.AddDays(-5), // Past drive
                        ApplicationDeadline = DateTime.Today.AddDays(-12),
                        NumberOfVacancies = 10,
                        Status = "Closed",
                        CreatedAt = DateTime.Now.AddDays(-25)
                    }
                };

                context.PlacementDrives.AddRange(drives);
                await context.SaveChangesAsync();
            }

            // 5. Seed Applications & Placements
            if (!await context.Applications.AnyAsync())
            {
                var students = await context.Students.ToListAsync();
                var drives = await context.PlacementDrives.Include(d => d.Company).ToListAsync();

                var aarav = students.First(s => s.EnrollmentNo == "21BCSE001");
                var diya = students.First(s => s.EnrollmentNo == "21BCSE045");
                var rohan = students.First(s => s.EnrollmentNo == "21BIT012");
                var meera = students.First(s => s.EnrollmentNo == "21BECE030");

                var msftDrive = drives.First(d => d.Company!.CompanyName.Contains("Microsoft"));
                var gsDrive = drives.First(d => d.Company!.CompanyName.Contains("Goldman Sachs"));
                var tcsDrive = drives.First(d => d.Company!.CompanyName.Contains("TCS"));
                var infosysDrive = drives.First(d => d.Company!.CompanyName.Contains("Infosys"));

                var applications = new List<Application>
                {
                    new Application
                    {
                        StudentId = diya.Id,
                        PlacementDriveId = msftDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-7),
                        Status = "Selected",
                        Remarks = "Cleared all technical rounds and HR interview with exceptional rating."
                    },
                    new Application
                    {
                        StudentId = aarav.Id,
                        PlacementDriveId = msftDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-7),
                        Status = "Interview",
                        Remarks = "Round 2 technical interview scheduled."
                    },
                    new Application
                    {
                        StudentId = aarav.Id,
                        PlacementDriveId = gsDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-6),
                        Status = "Shortlisted",
                        Remarks = "Shortlisted based on online assessment score (94%)."
                    },
                    new Application
                    {
                        StudentId = rohan.Id,
                        PlacementDriveId = tcsDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-5),
                        Status = "Selected",
                        Remarks = "Offered Systems Engineer role."
                    },
                    new Application
                    {
                        StudentId = meera.Id,
                        PlacementDriveId = tcsDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-4),
                        Status = "Applied",
                        Remarks = "Application submitted."
                    },
                    new Application
                    {
                        StudentId = aarav.Id,
                        PlacementDriveId = infosysDrive.Id,
                        ApplicationDate = DateTime.Now.AddDays(-3),
                        Status = "Applied",
                        Remarks = "Application submitted."
                    }
                };

                context.Applications.AddRange(applications);
                await context.SaveChangesAsync();

                // Seed Placements for the Selected students
                var placements = new List<Placement>
                {
                    new Placement
                    {
                        StudentId = diya.Id,
                        CompanyId = msftDrive.CompanyId,
                        PlacementDriveId = msftDrive.Id,
                        PlacementDate = DateTime.Today.AddDays(-2),
                        Package = msftDrive.SalaryPackage,
                        JobRole = msftDrive.JobTitle,
                        Status = "Placed"
                    },
                    new Placement
                    {
                        StudentId = rohan.Id,
                        CompanyId = tcsDrive.CompanyId,
                        PlacementDriveId = tcsDrive.Id,
                        PlacementDate = DateTime.Today.AddDays(-1),
                        Package = tcsDrive.SalaryPackage,
                        JobRole = tcsDrive.JobTitle,
                        Status = "Placed"
                    }
                };

                context.Placements.AddRange(placements);
                await context.SaveChangesAsync();
            }
        }
    }
}
