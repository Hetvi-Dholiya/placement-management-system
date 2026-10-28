-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: placement_management_db
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `placement_management_db`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `placement_management_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `placement_management_db`;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20261028050059_InitialCreate','9.0.0');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `adminusers`
--

DROP TABLE IF EXISTS `adminusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `adminusers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FullName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `adminusers`
--

LOCK TABLES `adminusers` WRITE;
/*!40000 ALTER TABLE `adminusers` DISABLE KEYS */;
INSERT INTO `adminusers` VALUES (1,'College Placement Director','admin@college.com','$2a$10$8wrhqZC6QfgH8CrQqMnD7OX0ivAiH4KaD/.ROWbxgkS8BuFEJs26C','Admin','2026-10-28 11:08:52.544053');
/*!40000 ALTER TABLE `adminusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `applications`
--

DROP TABLE IF EXISTS `applications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `applications` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `PlacementDriveId` int NOT NULL,
  `ApplicationDate` datetime(6) NOT NULL,
  `Status` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Applications_StudentId_PlacementDriveId` (`StudentId`,`PlacementDriveId`),
  KEY `IX_Applications_PlacementDriveId` (`PlacementDriveId`),
  CONSTRAINT `FK_Applications_PlacementDrives_PlacementDriveId` FOREIGN KEY (`PlacementDriveId`) REFERENCES `placementdrives` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Applications_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applications`
--

LOCK TABLES `applications` WRITE;
/*!40000 ALTER TABLE `applications` DISABLE KEYS */;
INSERT INTO `applications` VALUES (1,2,1,'2026-10-21 11:08:53.491845','Selected','Cleared all technical rounds and HR interview with exceptional rating.'),(2,1,1,'2026-10-21 11:08:53.492029','Interview','Round 2 technical interview scheduled.'),(3,1,2,'2026-10-22 11:08:53.492030','Shortlisted','Shortlisted based on online assessment score (94%).'),(4,3,3,'2026-10-23 11:08:53.492031','Selected','Offered Systems Engineer role.'),(5,4,3,'2026-10-24 11:08:53.492031','Applied','Application submitted.'),(6,1,4,'2026-10-25 11:08:53.492032','Applied','Application submitted.');
/*!40000 ALTER TABLE `applications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `companies`
--

DROP TABLE IF EXISTS `companies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `companies` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CompanyDescription` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Industry` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Website` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `HRName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `HREmail` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ContactNumber` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `companies`
--

LOCK TABLES `companies` WRITE;
/*!40000 ALTER TABLE `companies` DISABLE KEYS */;
INSERT INTO `companies` VALUES (1,'Tata Consultancy Services (TCS)','A global leader in IT services, consulting, and business solutions with a vast network of innovation and delivery centers.','IT Services & Consulting','Mumbai, Maharashtra','https://www.tcs.com','Vikram Sharma','vikram.sharma@tcs.com','9823456780','2026-08-29 11:08:52.818882'),(2,'Infosys Limited','A global leader in next-generation digital services and consulting, enabling clients in more than 50 countries to navigate digital transformation.','Software & Cloud Services','Bengaluru, Karnataka','https://www.infosys.com','Ananya Iyer','ananya.iyer@infosys.com','9876543210','2026-09-03 11:08:52.818975'),(3,'Microsoft India','Technology corporation producing computer software, consumer electronics, personal computers, and related enterprise cloud services.','Product & Technology','Hyderabad, Telangana','https://www.microsoft.com','Rajesh Gupta','rajesh.gupta@microsoft.com','9123456781','2026-09-08 11:08:52.818977'),(4,'Cognizant Technology','Leading professional services company transforming clients\' business, operating, and technology models for the digital era.','IT & Digital Engineering','Chennai, Tamil Nadu','https://www.cognizant.com','Pooja Hegde','pooja.hegde@cognizant.com','9812345672','2026-09-13 11:08:52.818977'),(5,'Goldman Sachs','Leading global financial institution providing investment banking, securities, investment management, and consumer banking services.','Fintech & Banking','Bengaluru, Karnataka','https://www.goldmansachs.com','Kunal Verma','kunal.verma@gs.com','9845612345','2026-09-18 11:08:52.818978'),(6,'Amazon Web Services','Comprehensive, evolving cloud computing platform provided by Amazon that offers a mixture of IaaS, PaaS and packaged software products.','Cloud & E-Commerce','Bengaluru, Karnataka','https://aws.amazon.com','Sneha Patel','sneha.patel@amazon.com','9898765432','2026-09-23 11:08:52.818982');
/*!40000 ALTER TABLE `companies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `placementdrives`
--

DROP TABLE IF EXISTS `placementdrives`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `placementdrives` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompanyId` int NOT NULL,
  `JobTitle` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `JobDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EligibilityCGPA` decimal(4,2) NOT NULL,
  `EligibleDepartment` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RequiredSkills` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SalaryPackage` decimal(5,2) NOT NULL,
  `JobLocation` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DriveDate` datetime(6) NOT NULL,
  `ApplicationDeadline` datetime(6) NOT NULL,
  `NumberOfVacancies` int NOT NULL,
  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PlacementDrives_CompanyId` (`CompanyId`),
  CONSTRAINT `FK_PlacementDrives_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `companies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `placementdrives`
--

LOCK TABLES `placementdrives` WRITE;
/*!40000 ALTER TABLE `placementdrives` DISABLE KEYS */;
INSERT INTO `placementdrives` VALUES (1,3,'Software Development Engineer - I','Design and build core cloud components for Azure infrastructure. Work with scalable microservices, high-throughput distributed systems, and real-time data pipelines.',8.50,'Computer Science and Engineering','Data Structures, Algorithms, C#/C++/Java, Cloud Architecture',22.50,'Hyderabad / Bengaluru','2026-11-11 00:00:00.000000','2026-11-04 00:00:00.000000',12,'Active','2026-10-18 11:08:53.276480'),(2,5,'Financial Technology Analyst','Build mission-critical algorithmic trading platforms, quantitative risk models, and enterprise financial transaction engines using modern tech stacks.',8.00,'All','Java, Python, SQL, Financial Algorithms, Analytical Reasoning',18.00,'Bengaluru','2026-11-17 00:00:00.000000','2026-11-07 00:00:00.000000',8,'Active','2026-10-20 11:08:53.276516'),(3,1,'Systems Engineer (Digital)','Full stack application development, API design, DevOps pipelines, and enterprise software engineering for international clients.',6.50,'All','C#, Java, Python, Web Technologies, Database Concepts',7.20,'Pan India','2026-11-22 00:00:00.000000','2026-11-09 00:00:00.000000',45,'Active','2026-10-22 11:08:53.276517'),(4,2,'Specialist Programmer (Power Programmer)','Core technology development focusing on high-level coding, machine learning, cloud application modernization, and automation platforms.',7.50,'Computer Science and Engineering','Advanced DSA, Competitive Programming, Full Stack Development',9.50,'Bengaluru / Pune','2026-11-27 00:00:00.000000','2026-11-12 00:00:00.000000',20,'Active','2026-10-23 11:08:53.276518'),(5,4,'Programmer Analyst Trainee (GenC Next)','Develop web applications, microservices, cloud deployments, and automated testing suites for digital enterprise solutions.',7.00,'All','OOPs, SQL, HTML, CSS, JavaScript, Problem Solving',6.75,'Chennai / Hyderabad','2026-11-15 00:00:00.000000','2026-11-06 00:00:00.000000',35,'Active','2026-10-24 11:08:53.276518'),(6,6,'Cloud Support Associate','Assist enterprise customers in architecting reliable cloud systems on AWS, troubleshooting distributed cloud incidents, and optimizing infrastructure.',7.20,'Information Technology','Networking, Linux, AWS Cloud Services, Python Scripting',11.00,'Bengaluru','2026-10-23 00:00:00.000000','2026-10-16 00:00:00.000000',10,'Closed','2026-10-03 11:08:53.276520');
/*!40000 ALTER TABLE `placementdrives` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `placements`
--

DROP TABLE IF EXISTS `placements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `placements` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `CompanyId` int NOT NULL,
  `PlacementDriveId` int NOT NULL,
  `PlacementDate` datetime(6) NOT NULL,
  `Package` decimal(5,2) NOT NULL,
  `JobRole` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Placements_CompanyId` (`CompanyId`),
  KEY `IX_Placements_PlacementDriveId` (`PlacementDriveId`),
  KEY `IX_Placements_StudentId` (`StudentId`),
  CONSTRAINT `FK_Placements_Companies_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `companies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Placements_PlacementDrives_PlacementDriveId` FOREIGN KEY (`PlacementDriveId`) REFERENCES `placementdrives` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Placements_Students_StudentId` FOREIGN KEY (`StudentId`) REFERENCES `students` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `placements`
--

LOCK TABLES `placements` WRITE;
/*!40000 ALTER TABLE `placements` DISABLE KEYS */;
INSERT INTO `placements` VALUES (1,2,3,1,'2026-10-26 00:00:00.000000',22.50,'Software Development Engineer - I','Placed'),(2,3,1,3,'2026-10-27 00:00:00.000000',7.20,'Systems Engineer (Digital)','Placed');
/*!40000 ALTER TABLE `placements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EnrollmentNo` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FullName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DateOfBirth` datetime(6) NOT NULL,
  `Gender` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Department` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Semester` int NOT NULL,
  `CGPA` decimal(4,2) NOT NULL,
  `Skills` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ResumeFileName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Students_Email` (`Email`),
  UNIQUE KEY `IX_Students_EnrollmentNo` (`EnrollmentNo`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES (1,'21BCSE001','Aarav Sharma','aarav.sharma@college.com','9876501234','2003-05-14 00:00:00.000000','Male','Computer Science and Engineering',8,8.85,'C#, ASP.NET Core, SQL, JavaScript, React',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-08 11:08:53.087737'),(2,'21BCSE045','Diya Patel','diya.patel@college.com','9823405678','2003-09-21 00:00:00.000000','Female','Computer Science and Engineering',8,9.20,'Java, Spring Boot, MySQL, Docker, Python',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-10 11:08:53.087764'),(3,'21BIT012','Rohan Verma','rohan.verma@college.com','9712345678','2002-12-10 00:00:00.000000','Male','Information Technology',8,7.65,'Python, Django, PostgreSQL, HTML5, CSS3',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-12 11:08:53.087764'),(4,'21BECE030','Meera Nair','meera.nair@college.com','9654321098','2003-03-18 00:00:00.000000','Female','Electronics and Communication',8,8.10,'C++, Embedded Systems, IoT, Python',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-16 11:08:53.087765'),(5,'21BCSE088','Kabir Singh','kabir.singh@college.com','9543216789','2002-08-30 00:00:00.000000','Male','Computer Science and Engineering',8,6.80,'JavaScript, Node.js, Express, MongoDB',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-20 11:08:53.087765'),(6,'22BCSE102','Ananya Roy','ananya.roy@college.com','9432167890','2004-01-15 00:00:00.000000','Female','Computer Science and Engineering',7,8.45,'C#, .NET MVC, Angular, SQL Server',NULL,'$2a$10$QczaxjXHfktVeUJj6KODuudxdgD36rHYDT3EgiszkIqQvdUo2569m','Student','2026-09-28 11:08:53.087771');
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-10-28 11:12:43
