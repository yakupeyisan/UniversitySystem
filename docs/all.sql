
-- 1️⃣ Tüm foreign key’leri kaldır
PRINT 'Dropping all foreign keys...';
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' +
               OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];'
FROM sys.foreign_keys;
EXEC sp_executesql @sql;

-- 2️⃣ Tüm stored procedure’leri sil
PRINT 'Dropping all stored procedures...';
DECLARE @procSql NVARCHAR(MAX) = '';
SELECT @procSql += 'DROP PROCEDURE [' + SCHEMA_NAME(schema_id) + '].[' + name + '];' + CHAR(13)
FROM sys.procedures;
EXEC sp_executesql @procSql;

-- 3️⃣ Tüm tabloları sil
PRINT 'Dropping all tables...';
EXEC sp_MSforeachtable 'DROP TABLE ?';

PRINT '✅ All tables and stored procedures have been dropped successfully.';


-- ============================================================
-- MULTI-BANK VIRTUAL POS SYSTEM - MSSQL DATABASE SCHEMA
-- ============================================================
-- Author: Generated from ER Diagram
-- Date: 2025-10-25
-- Description: Comprehensive schema for multi-bank virtual POS, wallet, and event management system
-- Version: FIXED & OPTIMIZED
-- ============================================================

-- ============================================================
-- DROP STORED PROCEDURES FIRST
-- ============================================================

IF OBJECT_ID('dbo.sp_GetPersonWalletBalance', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetPersonWalletBalance;
GO

IF OBJECT_ID('dbo.sp_RecordCardTransaction', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_RecordCardTransaction;
GO


-- ============================================================
-- DROP EXISTING TABLES (In Reverse Dependency Order)
-- ============================================================

IF OBJECT_ID('dbo.WorkflowApprovals', 'U') IS NOT NULL DROP TABLE dbo.WorkflowApprovals;
IF OBJECT_ID('dbo.WalletTransactionHistory', 'U') IS NOT NULL DROP TABLE dbo.WalletTransactionHistory;
IF OBJECT_ID('dbo.VirtualPosProviders', 'U') IS NOT NULL DROP TABLE dbo.VirtualPosProviders;
IF OBJECT_ID('dbo.VirtualPosIntegrations', 'U') IS NOT NULL DROP TABLE dbo.VirtualPosIntegrations;
IF OBJECT_ID('dbo.VehicleRegistration', 'U') IS NOT NULL DROP TABLE dbo.VehicleRegistration;
IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.TrainingSessions', 'U') IS NOT NULL DROP TABLE dbo.TrainingSessions;
IF OBJECT_ID('dbo.TrainingPrograms', 'U') IS NOT NULL DROP TABLE dbo.TrainingPrograms;
IF OBJECT_ID('dbo.TicketReservations', 'U') IS NOT NULL DROP TABLE dbo.TicketReservations;
IF OBJECT_ID('dbo.TicketResales', 'U') IS NOT NULL DROP TABLE dbo.TicketResales;
IF OBJECT_ID('dbo.SystemSettings', 'U') IS NOT NULL DROP TABLE dbo.SystemSettings;
IF OBJECT_ID('dbo.SystemNotifications', 'U') IS NOT NULL DROP TABLE dbo.SystemNotifications;
IF OBJECT_ID('dbo.StudentFees', 'U') IS NOT NULL DROP TABLE dbo.StudentFees;
IF OBJECT_ID('dbo.StudentAssignments', 'U') IS NOT NULL DROP TABLE dbo.StudentAssignments;
IF OBJECT_ID('dbo.SettlementReports', 'U') IS NOT NULL DROP TABLE dbo.SettlementReports;
IF OBJECT_ID('dbo.SettlementDiscrepancies', 'U') IS NOT NULL DROP TABLE dbo.SettlementDiscrepancies;
IF OBJECT_ID('dbo.SeatReservations', 'U') IS NOT NULL DROP TABLE dbo.SeatReservations;
IF OBJECT_ID('dbo.Seats', 'U') IS NOT NULL DROP TABLE dbo.Seats;
IF OBJECT_ID('dbo.SeatingArrangement', 'U') IS NOT NULL DROP TABLE dbo.SeatingArrangement;
IF OBJECT_ID('dbo.ScholarshipDeductions', 'U') IS NOT NULL DROP TABLE dbo.ScholarshipDeductions;
IF OBJECT_ID('dbo.Scholarships', 'U') IS NOT NULL DROP TABLE dbo.Scholarships;
IF OBJECT_ID('dbo.Schedules', 'U') IS NOT NULL DROP TABLE dbo.Schedules;
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.ResearchPublications', 'U') IS NOT NULL DROP TABLE dbo.ResearchPublications;
IF OBJECT_ID('dbo.ResearchProjectMembers', 'U') IS NOT NULL DROP TABLE dbo.ResearchProjectMembers;
IF OBJECT_ID('dbo.ResearchProjects', 'U') IS NOT NULL DROP TABLE dbo.ResearchProjects;
IF OBJECT_ID('dbo.ResaleRestrictions', 'U') IS NOT NULL DROP TABLE dbo.ResaleRestrictions;
IF OBJECT_ID('dbo.RequestProcessingQueue', 'U') IS NOT NULL DROP TABLE dbo.RequestProcessingQueue;
IF OBJECT_ID('dbo.Refunds', 'U') IS NOT NULL DROP TABLE dbo.Refunds;
IF OBJECT_ID('dbo.Prerequisites', 'U') IS NOT NULL DROP TABLE dbo.Prerequisites;
IF OBJECT_ID('dbo.PrerequisiteWaivers', 'U') IS NOT NULL DROP TABLE dbo.PrerequisiteWaivers;
IF OBJECT_ID('dbo.PersonWalletTransactions', 'U') IS NOT NULL DROP TABLE dbo.PersonWalletTransactions;
IF OBJECT_ID('dbo.PersonWalletLock', 'U') IS NOT NULL DROP TABLE dbo.PersonWalletLock;
IF OBJECT_ID('dbo.PersonRestrictions', 'U') IS NOT NULL DROP TABLE dbo.PersonRestrictions;
IF OBJECT_ID('dbo.PersonAccessGroups', 'U') IS NOT NULL DROP TABLE dbo.PersonAccessGroups;
IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL DROP TABLE dbo.Permissions;
IF OBJECT_ID('dbo.PaymentTransactionsLog', 'U') IS NOT NULL DROP TABLE dbo.PaymentTransactionsLog;
IF OBJECT_ID('dbo.PaymentReversals', 'U') IS NOT NULL DROP TABLE dbo.PaymentReversals;
IF OBJECT_ID('dbo.PaymentProviderCredentials', 'U') IS NOT NULL DROP TABLE dbo.PaymentProviderCredentials;
IF OBJECT_ID('dbo.PaymentPlanInstallments', 'U') IS NOT NULL DROP TABLE dbo.PaymentPlanInstallments;
IF OBJECT_ID('dbo.PaymentPlans', 'U') IS NOT NULL DROP TABLE dbo.PaymentPlans;
IF OBJECT_ID('dbo.PaySlips', 'U') IS NOT NULL DROP TABLE dbo.PaySlips;
IF OBJECT_ID('dbo.ParkingTransactions', 'U') IS NOT NULL DROP TABLE dbo.ParkingTransactions;
IF OBJECT_ID('dbo.ParkingReservationUsage', 'U') IS NOT NULL DROP TABLE dbo.ParkingReservationUsage;
IF OBJECT_ID('dbo.ParkingReservation', 'U') IS NOT NULL DROP TABLE dbo.ParkingReservation;
IF OBJECT_ID('dbo.ParkingRateConfig', 'U') IS NOT NULL DROP TABLE dbo.ParkingRateConfig;
IF OBJECT_ID('dbo.ParkingLots', 'U') IS NOT NULL DROP TABLE dbo.ParkingLots;
IF OBJECT_ID('dbo.ParkingEntryExitLog', 'U') IS NOT NULL DROP TABLE dbo.ParkingEntryExitLog;
IF OBJECT_ID('dbo.ParkingCards', 'U') IS NOT NULL DROP TABLE dbo.ParkingCards;
IF OBJECT_ID('dbo.MessageRecipients', 'U') IS NOT NULL DROP TABLE dbo.MessageRecipients;
IF OBJECT_ID('dbo.Messages', 'U') IS NOT NULL DROP TABLE dbo.Messages;
IF OBJECT_ID('dbo.MealPlans', 'U') IS NOT NULL DROP TABLE dbo.MealPlans;
IF OBJECT_ID('dbo.MazeretRaporlari', 'U') IS NOT NULL DROP TABLE dbo.MazeretRaporlari;
IF OBJECT_ID('dbo.ManualCorrectionsLog', 'U') IS NOT NULL DROP TABLE dbo.ManualCorrectionsLog;
IF OBJECT_ID('dbo.LibraryReservationNotifications', 'U') IS NOT NULL DROP TABLE dbo.LibraryReservationNotifications;
IF OBJECT_ID('dbo.LibraryReservations', 'U') IS NOT NULL DROP TABLE dbo.LibraryReservations;
IF OBJECT_ID('dbo.LibraryLoans', 'U') IS NOT NULL DROP TABLE dbo.LibraryLoans;
IF OBJECT_ID('dbo.LibraryMaterials', 'U') IS NOT NULL DROP TABLE dbo.LibraryMaterials;
IF OBJECT_ID('dbo.LibraryFines', 'U') IS NOT NULL DROP TABLE dbo.LibraryFines;
IF OBJECT_ID('dbo.LibraryFineConfig', 'U') IS NOT NULL DROP TABLE dbo.LibraryFineConfig;
IF OBJECT_ID('dbo.LibraryBorrowings', 'U') IS NOT NULL DROP TABLE dbo.LibraryBorrowings;
IF OBJECT_ID('dbo.LibraryBooks', 'U') IS NOT NULL DROP TABLE dbo.LibraryBooks;
IF OBJECT_ID('dbo.LaboratoryExperiments', 'U') IS NOT NULL DROP TABLE dbo.LaboratoryExperiments;
IF OBJECT_ID('dbo.LaboratoryEquipment', 'U') IS NOT NULL DROP TABLE dbo.LaboratoryEquipment;
IF OBJECT_ID('dbo.LaboratoryBookings', 'U') IS NOT NULL DROP TABLE dbo.LaboratoryBookings;
IF OBJECT_ID('dbo.Laboratory', 'U') IS NOT NULL DROP TABLE dbo.Laboratory;
IF OBJECT_ID('dbo.HourlyAccessSummary', 'U') IS NOT NULL DROP TABLE dbo.HourlyAccessSummary;
IF OBJECT_ID('dbo.HealthRecords', 'U') IS NOT NULL DROP TABLE dbo.HealthRecords;
IF OBJECT_ID('dbo.GraduationInfo', 'U') IS NOT NULL DROP TABLE dbo.GraduationInfo;
IF OBJECT_ID('dbo.GradingScaleConfig', 'U') IS NOT NULL DROP TABLE dbo.GradingScaleConfig;
IF OBJECT_ID('dbo.GradeObjections', 'U') IS NOT NULL DROP TABLE dbo.GradeObjections;
IF OBJECT_ID('dbo.Grades', 'U') IS NOT NULL DROP TABLE dbo.Grades;
IF OBJECT_ID('dbo.FileAttachments', 'U') IS NOT NULL DROP TABLE dbo.FileAttachments;
IF OBJECT_ID('dbo.Fees', 'U') IS NOT NULL DROP TABLE dbo.Fees;
IF OBJECT_ID('dbo.ExamConflictLog', 'U') IS NOT NULL DROP TABLE dbo.ExamConflictLog;
IF OBJECT_ID('dbo.Exams', 'U') IS NOT NULL DROP TABLE dbo.Exams;
IF OBJECT_ID('dbo.EventRefundPolicies', 'U') IS NOT NULL DROP TABLE dbo.EventRefundPolicies;
IF OBJECT_ID('dbo.EventRealTimeCapacity', 'U') IS NOT NULL DROP TABLE dbo.EventRealTimeCapacity;
IF OBJECT_ID('dbo.EventCheckIns', 'U') IS NOT NULL DROP TABLE dbo.EventCheckIns;
IF OBJECT_ID('dbo.EventTickets', 'U') IS NOT NULL DROP TABLE dbo.EventTickets;
IF OBJECT_ID('dbo.EventCancellations', 'U') IS NOT NULL DROP TABLE dbo.EventCancellations;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Venue', 'U') IS NOT NULL DROP TABLE dbo.Venue;
IF OBJECT_ID('dbo.Equipment', 'U') IS NOT NULL DROP TABLE dbo.Equipment;
IF OBJECT_ID('dbo.EmergencyContacts', 'U') IS NOT NULL DROP TABLE dbo.EmergencyContacts;
IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL DROP TABLE dbo.Documents;
IF OBJECT_ID('dbo.DeviceOfflineBuffer', 'U') IS NOT NULL DROP TABLE dbo.DeviceOfflineBuffer;
IF OBJECT_ID('dbo.DeviceHeartbeat', 'U') IS NOT NULL DROP TABLE dbo.DeviceHeartbeat;
IF OBJECT_ID('dbo.DeviceConfig', 'U') IS NOT NULL DROP TABLE dbo.DeviceConfig;
IF OBJECT_ID('dbo.DailyAccessSummary', 'U') IS NOT NULL DROP TABLE dbo.DailyAccessSummary;
IF OBJECT_ID('dbo.CurriculumCourses', 'U') IS NOT NULL DROP TABLE dbo.CurriculumCourses;
IF OBJECT_ID('dbo.Curricula', 'U') IS NOT NULL DROP TABLE dbo.Curricula;
IF OBJECT_ID('dbo.CourseWaitingList', 'U') IS NOT NULL DROP TABLE dbo.CourseWaitingList;
IF OBJECT_ID('dbo.CourseSessions', 'U') IS NOT NULL DROP TABLE dbo.CourseSessions;
IF OBJECT_ID('dbo.CourseRegistrations', 'U') IS NOT NULL DROP TABLE dbo.CourseRegistrations;
IF OBJECT_ID('dbo.CourseMaterials', 'U') IS NOT NULL DROP TABLE dbo.CourseMaterials;
IF OBJECT_ID('dbo.CourseEnrollments', 'U') IS NOT NULL DROP TABLE dbo.CourseEnrollments;
IF OBJECT_ID('dbo.CourseCapacityConfig', 'U') IS NOT NULL DROP TABLE dbo.CourseCapacityConfig;
IF OBJECT_ID('dbo.CourseAssignments', 'U') IS NOT NULL DROP TABLE dbo.CourseAssignments;
IF OBJECT_ID('dbo.CourseAnnouncements', 'U') IS NOT NULL DROP TABLE dbo.CourseAnnouncements;
IF OBJECT_ID('dbo.CorrectionApprovalRequests', 'U') IS NOT NULL DROP TABLE dbo.CorrectionApprovalRequests;
IF OBJECT_ID('dbo.ClubMembers', 'U') IS NOT NULL DROP TABLE dbo.ClubMembers;
IF OBJECT_ID('dbo.ClubEvents', 'U') IS NOT NULL DROP TABLE dbo.ClubEvents;
IF OBJECT_ID('dbo.StudentClubs', 'U') IS NOT NULL DROP TABLE dbo.StudentClubs;
IF OBJECT_ID('dbo.ClassroomEquipment', 'U') IS NOT NULL DROP TABLE dbo.ClassroomEquipment;
IF OBJECT_ID('dbo.Classrooms', 'U') IS NOT NULL DROP TABLE dbo.Classrooms;
IF OBJECT_ID('dbo.Cards', 'U') IS NOT NULL DROP TABLE dbo.Cards;
IF OBJECT_ID('dbo.CafeteriaSubscriptions', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaSubscriptions;
IF OBJECT_ID('dbo.CafeteriaPricingRules', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaPricingRules;
IF OBJECT_ID('dbo.CafeteriaMenus', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaMenus;
IF OBJECT_ID('dbo.CafeteriaInventory', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaInventory;
IF OBJECT_ID('dbo.CafeteriaDailyUsage', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaDailyUsage;
IF OBJECT_ID('dbo.CafeteriaAccessLogs', 'U') IS NOT NULL DROP TABLE dbo.CafeteriaAccessLogs;
IF OBJECT_ID('dbo.Cafeterias', 'U') IS NOT NULL DROP TABLE dbo.Cafeterias;
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
IF OBJECT_ID('dbo.Attendances', 'U') IS NOT NULL DROP TABLE dbo.Attendances;
IF OBJECT_ID('dbo.Courses', 'U') IS NOT NULL DROP TABLE dbo.Courses;
IF OBJECT_ID('dbo.AnomalyAlerts', 'U') IS NOT NULL DROP TABLE dbo.AnomalyAlerts;
IF OBJECT_ID('dbo.Announcements', 'U') IS NOT NULL DROP TABLE dbo.Announcements;
IF OBJECT_ID('dbo.AdvisoryNotes', 'U') IS NOT NULL DROP TABLE dbo.AdvisoryNotes;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Staff', 'U') IS NOT NULL DROP TABLE dbo.Staff;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Department', 'U') IS NOT NULL DROP TABLE dbo.Department;
IF OBJECT_ID('dbo.Faculty', 'U') IS NOT NULL DROP TABLE dbo.Faculty;
IF OBJECT_ID('dbo.Campus', 'U') IS NOT NULL DROP TABLE dbo.Campus;
IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL DROP TABLE dbo.Addresses;
IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL DROP TABLE dbo.Persons;
IF OBJECT_ID('dbo.AccessScheduleExceptions', 'U') IS NOT NULL DROP TABLE dbo.AccessScheduleExceptions;
IF OBJECT_ID('dbo.AccessPoints', 'U') IS NOT NULL DROP TABLE dbo.AccessPoints;
IF OBJECT_ID('dbo.AccessLogs', 'U') IS NOT NULL DROP TABLE dbo.AccessLogs;
IF OBJECT_ID('dbo.AccessGroupPermissions', 'U') IS NOT NULL DROP TABLE dbo.AccessGroupPermissions;
IF OBJECT_ID('dbo.AccessGroups', 'U') IS NOT NULL DROP TABLE dbo.AccessGroups;
IF OBJECT_ID('dbo.AccessControlDevices', 'U') IS NOT NULL DROP TABLE dbo.AccessControlDevices;
IF OBJECT_ID('dbo.AccessControlChannels', 'U') IS NOT NULL DROP TABLE dbo.AccessControlChannels;
GO

-- ============================================================
-- CREATE ALL TABLES (Dependency Order)
-- ============================================================

CREATE TABLE dbo.AccessControlChannels (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChannelName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    Location NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AccessControlDevices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ChannelId UNIQUEIDENTIFIER NOT NULL,
    DeviceName NVARCHAR(100) NOT NULL,
    DeviceType INT NOT NULL,
    Location NVARCHAR(200) NULL,
    IPAddress NVARCHAR(45) NULL,
    Status INT NOT NULL DEFAULT 0,
    LastHeartbeat DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AccessGroups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AccessGroupPermissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccessGroupId UNIQUEIDENTIFIER NOT NULL,
    ChannelId UNIQUEIDENTIFIER NOT NULL,
    AllowAccess BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(AccessGroupId, ChannelId)
);
GO
CREATE TABLE dbo.AccessLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CardId UNIQUEIDENTIFIER NOT NULL,
    EntryExitType INT NOT NULL, -- Entry, Exit
    EntryMethod INT NOT NULL, -- Card, Manual, Biometric
    Location NVARCHAR(100) NOT NULL,
    AllowedStatus BIT NOT NULL DEFAULT 1,
    DenialReason NVARCHAR(MAX) NULL,
    IPAddress NVARCHAR(45) NULL,
    DeviceId NVARCHAR(100) NULL,
    ManuallyCreatedByUserId UNIQUEIDENTIFIER NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL,
    CancellationReason NVARCHAR(MAX) NULL,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancelledAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AccessPoints (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PointName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    Location NVARCHAR(200) NOT NULL,
    AccessType INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AccessScheduleExceptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccessGroupId UNIQUEIDENTIFIER NOT NULL,
    ExceptionDate DATE NOT NULL,
    AllowAccess BIT NOT NULL DEFAULT 0,
    Reason NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Persons (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    DateOfBirth DATE NULL,
    Gender CHAR(1) NULL, -- M, F, O
    Email NVARCHAR(100) UNIQUE NULL,
    PhoneNumber NVARCHAR(20) NULL,
    IdNumber NVARCHAR(50) UNIQUE NULL,
    BloodType NVARCHAR(5) NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Inactive, Graduated, Suspended
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Addresses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    AddressType INT NOT NULL, -- Home, Work, Billing, Shipping, Other
    StreetAddress NVARCHAR(MAX) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    IsDefault BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Campus (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CampusName NVARCHAR(200) NOT NULL UNIQUE,
    Location NVARCHAR(MAX) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Faculty (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CampusId UNIQUEIDENTIFIER NOT NULL,
    FacultyName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DeanName NVARCHAR(100) NULL,
    DeanEmail NVARCHAR(100) NULL,
    Phone NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(CampusId, FacultyName)
);
GO
CREATE TABLE dbo.Department (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FacultyId UNIQUEIDENTIFIER NOT NULL,
    DepartmentName NVARCHAR(200) NOT NULL,
    DepartmentCode NVARCHAR(20) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ChairmanName NVARCHAR(100) NULL,
    ChairmanEmail NVARCHAR(100) NULL,
    Phone NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(FacultyId, DepartmentCode)
);

CREATE INDEX IX_Campus_CampusName ON dbo.Campus(CampusName);
CREATE INDEX IX_Faculty_CampusId ON dbo.Faculty(CampusId);
CREATE INDEX IX_Faculty_FacultyName ON dbo.Faculty(FacultyName);
CREATE INDEX IX_Department_FacultyId ON dbo.Department(FacultyId);
CREATE INDEX IX_Department_DepartmentCode ON dbo.Department(DepartmentCode);
GO
CREATE TABLE dbo.Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Role INT NOT NULL CHECK (Role IN (0,1,2,3,4,5)), -- Admin, Manager, Staff, Student, Finance, Support
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Staff (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    EmployeeNo NVARCHAR(50) NOT NULL UNIQUE,
    DepartmentId UNIQUEIDENTIFIER NULL,
    Title NVARCHAR(100) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    Salary DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, OnLeave, Retired, Terminated
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Students (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL UNIQUE FOREIGN KEY REFERENCES dbo.Persons(Id),
    StudentNo NVARCHAR(50) NOT NULL UNIQUE,
    EnrollmentDate DATE NOT NULL,
    CampusId UNIQUEIDENTIFIER NOT NULL,
    FacultyId UNIQUEIDENTIFIER NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,
    Major NVARCHAR(100) NOT NULL,
    ClassYear INT NOT NULL, -- 1, 2, 3, 4
    Status INT NOT NULL DEFAULT 0, -- Active, Academic Probation, Suspended, Graduated, Withdrawn
    GPA DECIMAL(3,2) NULL,
    TotalCredits INT NOT NULL DEFAULT 0,
    AdvisorStaffId UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Students_StudentNo ON dbo.Students(StudentNo);
CREATE INDEX IX_Students_PersonId ON dbo.Students(PersonId);
CREATE INDEX IX_Students_CampusId ON dbo.Students(CampusId);
CREATE INDEX IX_Students_FacultyId ON dbo.Students(FacultyId);
CREATE INDEX IX_Students_DepartmentId ON dbo.Students(DepartmentId);
GO
CREATE TABLE dbo.AdvisoryNotes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    AdvisorId UNIQUEIDENTIFIER NOT NULL,
    NoteContent NVARCHAR(MAX) NOT NULL,
    NoteDate DATE NOT NULL,
    IsConfidential BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Documents & Files
GO
CREATE TABLE dbo.Announcements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    PublishedDate DATETIME2 NOT NULL,
    ExpiryDate DATETIME2 NULL,
    Priority INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AnomalyAlerts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AlertType INT NOT NULL,
    PersonId UNIQUEIDENTIFIER NULL,
    DeviceId UNIQUEIDENTIFIER NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Severity INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ResolvedAt DATETIME2 NULL
);

-- Academic System Extensions (50+ tables)
GO
CREATE TABLE dbo.Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseCode NVARCHAR(50) NOT NULL UNIQUE,
    CourseName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Instructor NVARCHAR(100) NOT NULL,
    Credits INT NOT NULL,
    Level INT NOT NULL, -- Undergraduate, Graduate
    Semester NVARCHAR(20) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MaxStudents INT NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Open, Closed, Cancelled
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Attendances (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    SessionId UNIQUEIDENTIFIER NOT NULL,
    AttendanceStatus INT NOT NULL,
    CheckInTime DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId UNIQUEIDENTIFIER NOT NULL,
    Action NVARCHAR(50) NOT NULL, -- Create, Update, Delete, Read
    OldValues NVARCHAR(MAX) NULL, -- JSON
    NewValues NVARCHAR(MAX) NULL, -- JSON
    IPAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Cafeterias (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaName NVARCHAR(100) NOT NULL UNIQUE,
    Location NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20) NULL,
    OperatingHours NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CafeteriaAccessLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CardId UNIQUEIDENTIFIER NOT NULL,
    AccessType INT NOT NULL, -- Breakfast, Lunch, Dinner, Snack
    AccessTime DATETIME2 NOT NULL,
    AllowedStatus BIT NOT NULL DEFAULT 1,
    DenialReason NVARCHAR(MAX) NULL,
    MealValue DECIMAL(18,4) NULL,
    DeviceId NVARCHAR(100) NULL,
    ManuallyCreatedByUserId UNIQUEIDENTIFIER NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL,
    CancellationReason NVARCHAR(MAX) NULL,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancelledAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CafeteriaDailyUsage (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId UNIQUEIDENTIFIER NOT NULL,
    UsageDate DATE NOT NULL,
    BreakfastCount INT NOT NULL DEFAULT 0,
    LunchCount INT NOT NULL DEFAULT 0,
    DinnerCount INT NOT NULL DEFAULT 0,
    TotalRevenue DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(CafeteriaId, UsageDate)
);
GO
CREATE TABLE dbo.CafeteriaInventory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId UNIQUEIDENTIFIER NOT NULL,
    ItemName NVARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    ReorderLevel INT NOT NULL,
    LastRestockDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CafeteriaMenus (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MenuName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    MealType INT NOT NULL, -- Breakfast, Lunch, Dinner, Snack
    Price DECIMAL(18,4) NOT NULL,
    DayOfWeek INT NOT NULL, -- 0=Sunday, 1=Monday, ... 6=Saturday
    AvailableFrom TIME NOT NULL,
    AvailableTo TIME NOT NULL,
    DailyQuota INT NOT NULL,
    RemainingQuota INT NOT NULL,
    Allergens NVARCHAR(MAX) NULL,
    IsVegetarian BIT NOT NULL DEFAULT 0,
    IsVegan BIT NOT NULL DEFAULT 0,
    Ingredients NVARCHAR(MAX) NULL,
    Calories INT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Discontinued
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CafeteriaPricingRules (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId UNIQUEIDENTIFIER NOT NULL,
    UserType INT NOT NULL,
    MealType INT NOT NULL,
    Price DECIMAL(18,4) NOT NULL,
    EffectiveDate DATE NOT NULL,
    EndDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CafeteriaSubscriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    SubscriptionType INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Price DECIMAL(18,4) NOT NULL,
    MealsUsed INT NOT NULL DEFAULT 0,
    MealsAllowed INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Communication & Notifications
GO
CREATE TABLE dbo.Cards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CardNumber NVARCHAR(50) NOT NULL UNIQUE,
    CardType INT NOT NULL, -- Student, Staff, Campus
    Balance DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    LastRechargeAmount DECIMAL(18,4) NULL,
    LastRechargeDate DATETIME2 NULL,
    ExpiryDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Blocked, Expired, Suspended
    IsRFIDEnabled BIT NOT NULL DEFAULT 0,
    RFIDCode NVARCHAR(100) NULL,
    Notes NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Classrooms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClassroomName NVARCHAR(100) NOT NULL UNIQUE,
    BuildingName NVARCHAR(100) NOT NULL,
    FloorNumber INT NULL,
    RoomNumber NVARCHAR(20) NOT NULL,
    Capacity INT NOT NULL,
    Location NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ClassroomEquipment (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClassroomId UNIQUEIDENTIFIER NOT NULL,
    EquipmentType NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.StudentClubs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClubName NVARCHAR(200) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    PresidentId UNIQUEIDENTIFIER NOT NULL,
    FoundedDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ClubEvents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClubId UNIQUEIDENTIFIER NOT NULL,
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    EventDate DATETIME2 NOT NULL,
    Location NVARCHAR(200) NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ClubMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClubId UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    Role NVARCHAR(50) NULL,
    JoinDate DATE NOT NULL,
    LeaveDate DATE NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(ClubId, StudentId)
);
GO
CREATE TABLE dbo.CorrectionApprovalRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ManualCorrectionLogId UNIQUEIDENTIFIER NOT NULL,
    ApprovedByUserId UNIQUEIDENTIFIER NOT NULL,
    ApprovalStatus INT NOT NULL DEFAULT 0, -- Pending, Approved, Rejected
    ApprovalNotes NVARCHAR(MAX) NULL,
    ApprovedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseAnnouncements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Priority INT NOT NULL DEFAULT 0,
    IsPublished BIT NOT NULL DEFAULT 1,
    PublishedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseAssignments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    AssignmentName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DueDate DATETIME2 NOT NULL,
    MaxPoints INT NOT NULL,
    IsPublished BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseCapacityConfig (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    MaxCapacity INT NOT NULL,
    MinCapacity INT NOT NULL,
    Priority INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseEnrollments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL,
    Grade INT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Completed, Dropped
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseMaterials (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    MaterialType INT NOT NULL, -- Lecture, Assignment, Reading, Video
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    FileURL NVARCHAR(500) NULL,
    UploadedDate DATETIME2 NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseRegistrations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate DATETIME2 NOT NULL,
    DropDate DATETIME2 NULL,
    Status INT NOT NULL DEFAULT 0,
    Grade NVARCHAR(5) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(StudentId, CourseId)
);
GO
CREATE TABLE dbo.CourseSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    SessionDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Location NVARCHAR(200) NULL,
    Instructor NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CourseWaitingList (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    PositionNumber INT NOT NULL,
    AddedDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(StudentId, CourseId)
);
GO
CREATE TABLE dbo.Curricula (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CurriculumName NVARCHAR(200) NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,
    Version INT NOT NULL DEFAULT 1,
    EffectiveDate DATE NOT NULL,
    Description NVARCHAR(MAX) NULL,
    TotalCredits INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.CurriculumCourses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CurriculumId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Semester INT NOT NULL,
    IsElective BIT NOT NULL DEFAULT 0,
    IsRequired BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(CurriculumId, CourseId)
);
GO
CREATE TABLE dbo.DailyAccessSummary (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SummaryDate DATE NOT NULL,
    TotalEntries INT NOT NULL DEFAULT 0,
    TotalExits INT NOT NULL DEFAULT 0,
    UnauthorizedAttempts INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(SummaryDate)
);
GO
CREATE TABLE dbo.DeviceConfig (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DeviceId UNIQUEIDENTIFIER NOT NULL,
    ConfigKey NVARCHAR(100) NOT NULL,
    ConfigValue NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(DeviceId, ConfigKey)
);
GO
CREATE TABLE dbo.DeviceHeartbeat (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DeviceId UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL,
    Message NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.DeviceOfflineBuffer (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DeviceId UNIQUEIDENTIFIER NOT NULL,
    TransactionData NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProcessedAt DATETIME2 NULL
);
GO
CREATE TABLE dbo.Documents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DocumentName NVARCHAR(200) NOT NULL,
    DocumentType NVARCHAR(50) NOT NULL,
    PersonId UNIQUEIDENTIFIER NOT NULL,
    DocumentDate DATE NOT NULL,
    FileURL NVARCHAR(500) NOT NULL,
    IsPublic BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EmergencyContacts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    ContactName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Relationship NVARCHAR(50) NOT NULL, -- Parent, Spouse, Sibling, Friend, Other
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(MAX) NULL,
    Priority INT NOT NULL DEFAULT 1, -- 1=Primary, 2=Secondary, 3=Tertiary
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Addresses_PersonId ON dbo.Addresses(PersonId);
CREATE INDEX IX_Addresses_AddressType ON dbo.Addresses(AddressType);
CREATE INDEX IX_EmergencyContacts_PersonId ON dbo.EmergencyContacts(PersonId);
CREATE INDEX IX_EmergencyContacts_Priority ON dbo.EmergencyContacts(Priority);
GO
CREATE TABLE dbo.Equipment (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EquipmentName NVARCHAR(200) NOT NULL,
    EquipmentType NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NULL,
    Status INT NOT NULL DEFAULT 0,
    PurchaseDate DATE NULL,
    LastMaintenanceDate DATE NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Venue (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VenueName NVARCHAR(200) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    TotalCapacity INT NOT NULL,
    Building NVARCHAR(50) NULL,
    FloorNumber INT NULL,
    TotalRows INT NOT NULL,
    TotalColumns INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Description NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Events (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    VenueId UNIQUEIDENTIFIER NOT NULL,
    OrganizerId UNIQUEIDENTIFIER NOT NULL,
    EventType INT NOT NULL, -- Concert, Theater, Conference, Exhibition, Workshop, Other
    EventDate DATETIME2 NOT NULL,
    EventStartTime TIME NOT NULL,
    EventEndTime TIME NOT NULL,
    TotalCapacity INT NOT NULL,
    AvailableSeats INT NOT NULL,
    DefaultTicketPrice DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Planning, TicketsOnSale, EventDay, Completed, Cancelled
    RequiresCheckIn BIT NOT NULL DEFAULT 0,
    PosterImagePath NVARCHAR(500) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EventCancellations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    CancelledAt DATETIME2 NOT NULL,
    CancellationReason NVARCHAR(MAX) NOT NULL,
    RefundMethod INT NOT NULL, -- FullRefund, Voucher, FutureEvent
    RefundProcessedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EventTickets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    SeatId UNIQUEIDENTIFIER NOT NULL,
    BuyerId UNIQUEIDENTIFIER NOT NULL,
    CardId UNIQUEIDENTIFIER NULL,
    TicketPrice DECIMAL(18,4) NOT NULL,
    PurchaseDate DATETIME2 NOT NULL,
    SaleMethod INT NOT NULL, -- Card, CashPayment, WebPayment, VirtualPOS
    VirtualPosProviderId UNIQUEIDENTIFIER NULL,
    PaymentTransactionLogId UNIQUEIDENTIFIER NULL,
    TicketCode NVARCHAR(100) NOT NULL UNIQUE,
    Status INT NOT NULL DEFAULT 0, -- Valid, Used, Refunded, Cancelled
    CheckInTime DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EventCheckIns (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventTicketId UNIQUEIDENTIFIER NOT NULL,
    CheckInTime DATETIME2 NOT NULL,
    CheckInMethod INT NOT NULL, -- QRCode, Manual, RFID
    CheckInPersonId UNIQUEIDENTIFIER NOT NULL,
    SeatConfirmation BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EventRealTimeCapacity (
    EventId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES dbo.Events(Id),
    CurrentOccupancy INT NOT NULL DEFAULT 0,
    LastUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.EventRefundPolicies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    DaysBeforeEvent INT NOT NULL,
    RefundPercentage DECIMAL(18,4) NOT NULL,
    ProcessingDays INT NOT NULL DEFAULT 3,
    IsRefundableAfterEventStart BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Exams (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    ExamName NVARCHAR(200) NOT NULL,
    ExamDate DATETIME2 NOT NULL,
    Duration INT NOT NULL,
    MaxScore INT NOT NULL,
    Location NVARCHAR(200) NULL,
    ExamType INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ExamConflictLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    Exam1Id UNIQUEIDENTIFIER NOT NULL,
    Exam2Id UNIQUEIDENTIFIER NOT NULL,
    ConflictDateTime DATETIME2 NOT NULL,
    ResolutionStatus INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Fees (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    FeeType INT NOT NULL,
    Amount DECIMAL(18,4) NOT NULL,
    DueDate DATE NOT NULL,
    PaidDate DATE NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.FileAttachments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParentEntityType NVARCHAR(100) NOT NULL,
    ParentEntityId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FileSize BIGINT NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    FileURL NVARCHAR(500) NOT NULL,
    UploadedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- System Configuration
GO
CREATE TABLE dbo.Grades (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Score DECIMAL(5,2) NOT NULL,
    LetterGrade NVARCHAR(2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(StudentId, CourseId)
);
GO
CREATE TABLE dbo.GradeObjections (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GradeId UNIQUEIDENTIFIER NOT NULL,
    ObjectionReason NVARCHAR(MAX) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    ReviewedBy UNIQUEIDENTIFIER NULL,
    ReviewedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.GradingScaleConfig (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DepartmentId UNIQUEIDENTIFIER NOT NULL,
    LetterGrade NVARCHAR(2) NOT NULL,
    MinScore DECIMAL(5,2) NOT NULL,
    MaxScore DECIMAL(5,2) NOT NULL,
    GradePoint DECIMAL(3,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(DepartmentId, LetterGrade)
);
GO
CREATE TABLE dbo.GraduationInfo (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL UNIQUE FOREIGN KEY REFERENCES dbo.Students(Id),
    GraduationDate DATE NOT NULL,
    Degree NVARCHAR(100) NOT NULL,
    GPA DECIMAL(3,2) NOT NULL,
    HonorsStatus INT NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.HealthRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    RecordType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ConfidentialNotes NVARCHAR(MAX) NULL,
    IsConfidential BIT NOT NULL DEFAULT 1,
    RecordDate DATE NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.HourlyAccessSummary (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SummaryDateTime DATETIME2 NOT NULL,
    ChannelId UNIQUEIDENTIFIER NOT NULL,
    TotalTransactions INT NOT NULL DEFAULT 0,
    SuccessfulTransactions INT NOT NULL DEFAULT 0,
    FailedTransactions INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(SummaryDateTime, ChannelId)
);
GO
CREATE TABLE dbo.Laboratory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabName NVARCHAR(100) NOT NULL UNIQUE,
    BuildingName NVARCHAR(100) NOT NULL,
    FloorNumber INT NULL,
    Capacity INT NOT NULL,
    Location NVARCHAR(200) NULL,
    LabType NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LaboratoryBookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    BookingDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LaboratoryEquipment (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabId UNIQUEIDENTIFIER NOT NULL,
    EquipmentName NVARCHAR(200) NOT NULL,
    SerialNumber NVARCHAR(100) NULL,
    Status INT NOT NULL DEFAULT 0,
    LastCalibrationDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LaboratoryExperiments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabId UNIQUEIDENTIFIER NOT NULL,
    ExperimentName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DurationMinutes INT NOT NULL,
    SafetyInstructions NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Library Extensions
GO
CREATE TABLE dbo.LibraryBooks (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ISBN NVARCHAR(20) NOT NULL UNIQUE,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100) NOT NULL,
    Publisher NVARCHAR(100) NOT NULL,
    PublicationYear INT NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    TotalCopies INT NOT NULL,
    AvailableCopies INT NOT NULL,
    Location NVARCHAR(100) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Available, Archived, Removed
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryBorrowings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    BookId UNIQUEIDENTIFIER NOT NULL,
    BorrowDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    ReturnDate DATE NULL,
    LateFee DECIMAL(18,4) NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Returned, Overdue
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryFineConfig (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FinePerDay DECIMAL(18,4) NOT NULL,
    MaxFineLimitPerItem DECIMAL(18,4) NOT NULL,
    GracePeriodDays INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Parking Extensions
GO
CREATE TABLE dbo.LibraryFines (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LoanId UNIQUEIDENTIFIER NOT NULL,
    FineAmount DECIMAL(18,4) NOT NULL,
    FineDate DATE NOT NULL,
    PaidDate DATE NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryMaterials (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ISBN NVARCHAR(20) UNIQUE NULL,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(200) NOT NULL,
    Publisher NVARCHAR(100) NULL,
    PublicationYear INT NULL,
    Category NVARCHAR(100) NOT NULL,
    TotalCopies INT NOT NULL,
    AvailableCopies INT NOT NULL,
    Location NVARCHAR(100) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryLoans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    LoanDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    ReturnDate DATE NULL,
    RenewalCount INT NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryReservations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    ReservationDate DATETIME2 NOT NULL,
    PickUpByDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.LibraryReservationNotifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReservationId UNIQUEIDENTIFIER NOT NULL,
    NotificationDate DATETIME2 NOT NULL,
    Method INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ManualCorrectionsLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RecordType INT NOT NULL, -- AccessLog, CafeteriaLog, ParkingLog
    OriginalRecordId UNIQUEIDENTIFIER NOT NULL,
    OperationType NVARCHAR(20) NOT NULL, -- Create, Update, Delete, Restore
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    OldValues NVARCHAR(MAX) NULL, -- JSON
    NewValues NVARCHAR(MAX) NULL, -- JSON
    ApprovalStatus INT NOT NULL DEFAULT 0, -- NoApprovalNeeded, PendingApproval, Approved, Rejected
    ApprovalRequestId UNIQUEIDENTIFIER NULL,
    RequiresApproval BIT NOT NULL DEFAULT 0,
    Details NVARCHAR(MAX) NULL, -- JSON
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO
CREATE TABLE dbo.MazeretRaporlari (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    Sebep NVARCHAR(MAX) NOT NULL,
    BaslangicTarihi DATE NOT NULL,
    BitisTarihi DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    OnaylayanId UNIQUEIDENTIFIER NULL,
    OnaylanmaTarihi DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.MealPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    PlanType INT NOT NULL, -- NoMeal, Breakfast, BreakfastLunch, Full
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MealsPerWeek INT NOT NULL,
    Price DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Inactive, Suspended
    Notes NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SenderId UNIQUEIDENTIFIER NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    MessageType INT NOT NULL,
    SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.MessageRecipients (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MessageId UNIQUEIDENTIFIER NOT NULL,
    RecipientId UNIQUEIDENTIFIER NOT NULL,
    ReadAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingCards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    LicensePlate NVARCHAR(50) NOT NULL UNIQUE,
    CardType INT NOT NULL, -- Daily, Monthly, Semester, Yearly
    StartDate DATE NOT NULL,
    ExpiryDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Expired, Suspended, Revoked
    Notes NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingEntryExitLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingCardId UNIQUEIDENTIFIER NOT NULL,
    ParkingLotId UNIQUEIDENTIFIER NOT NULL,
    EntryTime DATETIME2 NOT NULL,
    ExitTime DATETIME2 NULL,
    Duration INT NULL, -- in minutes
    ChargeAmount DECIMAL(18,4) NULL,
    PaymentStatus INT NOT NULL DEFAULT 0, -- NotPaid, Paid, Exempted
    ManuallyCreatedByUserId UNIQUEIDENTIFIER NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL,
    CancellationReason NVARCHAR(MAX) NULL,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancelledAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingLots (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingLotName NVARCHAR(100) NOT NULL UNIQUE,
    Location NVARCHAR(200) NOT NULL,
    TotalCapacity INT NOT NULL,
    AvailableSpots INT NOT NULL,
    PricePerHour DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Maintenance, Closed
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingRateConfig (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingLotId UNIQUEIDENTIFIER NOT NULL,
    RatePerHour DECIMAL(18,4) NOT NULL,
    RatePerDay DECIMAL(18,4) NOT NULL,
    MaxDailyRate DECIMAL(18,4) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingReservation (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    ParkingLotId UNIQUEIDENTIFIER NOT NULL,
    ReservationDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingReservationUsage (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReservationId UNIQUEIDENTIFIER NOT NULL,
    ActualCheckInTime DATETIME2 NULL,
    ActualCheckOutTime DATETIME2 NULL,
    ChargedAmount DECIMAL(18,4) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ParkingTransactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingCardId UNIQUEIDENTIFIER NOT NULL,
    EntryTime DATETIME2 NOT NULL,
    ExitTime DATETIME2 NULL,
    Duration INT NULL,
    ChargeAmount DECIMAL(18,4) NULL,
    PaymentStatus INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Cafeteria Extensions
GO
CREATE TABLE dbo.PaySlips (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StaffId UNIQUEIDENTIFIER NOT NULL,
    MonthYear NVARCHAR(10) NOT NULL, -- YYYY-MM format
    BasicSalary DECIMAL(18,4) NOT NULL,
    GrossSalary DECIMAL(18,4) NOT NULL,
    TotalDeductions DECIMAL(18,4) NOT NULL,
    NetSalary DECIMAL(18,4) NOT NULL,
    PaymentDate DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PaymentPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    TotalAmount DECIMAL(18,4) NOT NULL,
    NumberOfInstallments INT NOT NULL,
    StartDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PaymentPlanInstallments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PaymentPlanId UNIQUEIDENTIFIER NOT NULL,
    InstallmentNumber INT NOT NULL,
    DueAmount DECIMAL(18,4) NOT NULL,
    DueDate DATE NOT NULL,
    PaidDate DATE NULL,
    PaidAmount DECIMAL(18,4) NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PaymentProviderCredentials (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    MerchantId NVARCHAR(100) NOT NULL UNIQUE,
    MerchantKey NVARCHAR(MAX) NOT NULL, -- encrypted
    TerminalId NVARCHAR(100) NOT NULL UNIQUE,
    APIKey NVARCHAR(MAX) NOT NULL, -- encrypted
    APISecret NVARCHAR(MAX) NOT NULL, -- encrypted
    Username NVARCHAR(200) NULL, -- encrypted
    Password NVARCHAR(MAX) NULL, -- encrypted
    CredentialType NVARCHAR(20) NOT NULL, -- OAuth, APIKey, Basic, Custom
    ValidFrom DATETIME2 NOT NULL,
    ValidTo DATETIME2 NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0, -- Active, Inactive, Expired, NeedsRenewal
    CreatedBy UNIQUEIDENTIFIER NULL,
    LastUpdatedBy UNIQUEIDENTIFIER NULL,
    LastTestResult NVARCHAR(MAX) NULL,
    LastTestedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PaymentReversals (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OriginalTransactionLogId UNIQUEIDENTIFIER NOT NULL,
    ReversalReason INT NOT NULL, -- CustomerRequest, DuplicateTransaction, ErrorCorrection, ChargBack, SystemError
    ReversalAmount DECIMAL(18,4) NOT NULL,
    InitiatedBy UNIQUEIDENTIFIER NOT NULL,
    ReversalRequestDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Pending, Processing, Completed, Failed, Rejected
    BankReversalNo NVARCHAR(50) NULL,
    BankProcessingDate DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PaymentTransactionsLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TransactionType NVARCHAR(50) NOT NULL, -- CardRecharge, EventTicket, Parking, Cafeteria, StudentPayment
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    MerchantTransactionNo NVARCHAR(100) NOT NULL UNIQUE,
    TransactionReference NVARCHAR(100) NOT NULL UNIQUE,
    Amount DECIMAL(18,4) NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    Status INT NOT NULL, -- Pending, Processing, Authorized, Captured, Failed, Cancelled, Refunded
    BankResponseCode NVARCHAR(10) NULL,
    BankResponseMessage NVARCHAR(MAX) NULL,
    AuthorizationCode NVARCHAR(50) NULL,
    BatchNo NVARCHAR(50) NULL,
    ResponseTimeMs INT NULL,
    Settled BIT NOT NULL DEFAULT 0,
    SettlementDate DATETIME2 NULL,
    SettlementAmount DECIMAL(18,4) NULL,
    CommissionAmount DECIMAL(18,4) NULL,
    RequestPayload NVARCHAR(MAX) NULL, -- JSON - sanitized
    ResponsePayload NVARCHAR(MAX) NULL, -- JSON - sanitized
    IPAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PermissionName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PersonAccessGroups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    AccessGroupId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(PersonId, AccessGroupId)
);
GO
CREATE TABLE dbo.PersonRestrictions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    RestrictionType INT NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PersonWalletLock (
    PersonId UNIQUEIDENTIFIER PRIMARY KEY FOREIGN KEY REFERENCES dbo.Persons(Id),
    LockedAt DATETIME2 NOT NULL,
    LockedByProcessId NVARCHAR(100) NOT NULL,
    LockTimeoutSeconds INT NOT NULL DEFAULT 30,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PersonWalletTransactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CardId UNIQUEIDENTIFIER NULL,
    TransactionType INT NOT NULL, -- CardRecharge, CafeteriaPurchase, ParkingPayment, EventTicketPurchase, LibraryFine, etc.
    TransactionCategory INT NOT NULL, -- Income, Expense, Adjustment, Refund
    Amount DECIMAL(18,4) NOT NULL, -- Positive=Yükleme, Negative=Harcama
    BalanceBefore DECIMAL(18,4) NOT NULL,
    BalanceAfter DECIMAL(18,4) NOT NULL,
    TransactionSource INT NOT NULL, -- Turnstile, Cafeteria, Library, Parking, Classroom, WebPortal, etc.
    Description NVARCHAR(MAX) NULL,
    LocationName NVARCHAR(200) NULL,
    LocationCode NVARCHAR(50) NULL,
    ReferenceNo NVARCHAR(100) NOT NULL UNIQUE,
    LinkedEntityId UNIQUEIDENTIFIER NULL,
    LinkedEntityType NVARCHAR(100) NULL, -- Cafeteria, ParkingLot, Event, Course
    Status INT NOT NULL, -- Pending, Processing, Successful, Failed, Reversed, Cancelled
    FailureReason NVARCHAR(MAX) NULL,
    VirtualPosProviderId UNIQUEIDENTIFIER NULL,
    PaymentTransactionLogId UNIQUEIDENTIFIER NULL,
    PaymentMethod INT NOT NULL, -- Cash, Card, BankTransfer, VirtualPOS, Subscription, Internal, AdminCredit
    IsReconciled BIT NOT NULL DEFAULT 0,
    ReconciledAt DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    ApprovalNotes NVARCHAR(MAX) NULL,
    ApprovedBy UNIQUEIDENTIFIER NULL,
    ApprovedAt DATETIME2 NULL,
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.PrerequisiteWaivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    ApprovedBy UNIQUEIDENTIFIER NULL,
    ApprovedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Prerequisites (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    PrerequisiteCourseId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(CourseId, PrerequisiteCourseId)
);
GO
CREATE TABLE dbo.Refunds (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OriginalTransactionId UNIQUEIDENTIFIER NOT NULL,
    RefundAmount DECIMAL(18,4) NOT NULL,
    RefundReason NVARCHAR(MAX) NOT NULL,
    RequestedBy UNIQUEIDENTIFIER NOT NULL,
    ApprovedBy UNIQUEIDENTIFIER NULL,
    Status INT NOT NULL DEFAULT 0,
    ProcessedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.RequestProcessingQueue (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequestType NVARCHAR(100) NOT NULL,
    RequestData NVARCHAR(MAX) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    Priority INT NOT NULL DEFAULT 0,
    AttemptCount INT NOT NULL DEFAULT 0,
    LastAttemptAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ResaleRestrictions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    AllowResale BIT NOT NULL DEFAULT 0,
    MaxResaleMarkupPercent DECIMAL(18,4) NULL,
    MinDaysBeforeEventToResale INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ResearchProjects (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectName NVARCHAR(200) NOT NULL,
    ProjectNo NVARCHAR(50) NOT NULL UNIQUE,
    LeaderStaffId UNIQUEIDENTIFIER NOT NULL,
    ProjectType INT NOT NULL, -- TUBITAK, EU, Industry, InternalResearch
    Budget DECIMAL(18,4) NOT NULL,
    SpentAmount DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Planning, Active, Completed, Cancelled
    Description NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ResearchProjectMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    StaffId UNIQUEIDENTIFIER NOT NULL,
    Role INT NOT NULL, -- Manager, Researcher, Technician
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ResearchPublications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StaffId UNIQUEIDENTIFIER NOT NULL,
    ProjectId UNIQUEIDENTIFIER NULL,
    Title NVARCHAR(300) NOT NULL,
    Authors NVARCHAR(MAX) NOT NULL,
    PublicationVenue NVARCHAR(200) NOT NULL,
    PublicationType INT NOT NULL, -- Article, Book, Conference, Patent, Copyright
    PublicationDate DATETIME2 NOT NULL,
    DOI NVARCHAR(100) NULL,
    URL NVARCHAR(500) NULL,
    CitationCount INT NOT NULL DEFAULT 0,
    ImpactFactor FLOAT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.RolePermissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(RoleId, PermissionId)
);
GO
CREATE TABLE dbo.Schedules (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScheduleName NVARCHAR(200) NOT NULL,
    ScheduleType INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Workflow & Approvals
GO
CREATE TABLE dbo.Scholarships (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    ScholarshipName NVARCHAR(200) NOT NULL,
    Amount DECIMAL(18,4) NOT NULL,
    AwardDate DATE NOT NULL,
    EndDate DATE NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.ScholarshipDeductions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScholarshipId UNIQUEIDENTIFIER NOT NULL,
    DeductionReason NVARCHAR(MAX) NOT NULL,
    DeductionAmount DECIMAL(18,4) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SeatingArrangement (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VenueId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    TotalRows INT NOT NULL,
    TotalColumns INT NOT NULL,
    TotalSeats INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.Seats (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SeatingArrangementId UNIQUEIDENTIFIER NOT NULL,
    SeatNumber NVARCHAR(20) NOT NULL, -- e.g. A1, A2, B1
    RowNumber INT NOT NULL,
    ColumnNumber INT NOT NULL,
    SeatType INT NOT NULL, -- Normal, VIP, Handicap, StageView
    SeatPrice DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Available, ReservedByAdmin, SoldOut, Maintenance, Blocked
    Notes NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SeatReservations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SeatId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    ReservedForName NVARCHAR(100) NOT NULL,
    ReservationCode NVARCHAR(50) NOT NULL UNIQUE,
    ReservedBy UNIQUEIDENTIFIER NOT NULL,
    ReservationDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Used, Cancelled
    Notes NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SettlementDiscrepancies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SettlementReportId UNIQUEIDENTIFIER NOT NULL,
    OriginalTransactionLogId UNIQUEIDENTIFIER NOT NULL,
    OriginalTransactionAmount DECIMAL(18,4) NOT NULL,
    SettledAmount DECIMAL(18,4) NOT NULL,
    DiscrepancyAmount DECIMAL(18,4) NOT NULL,
    DiscrepancyReason NVARCHAR(MAX) NULL,
    Status INT NOT NULL DEFAULT 0, -- Reported, Investigating, Resolved
    ResolutionNotes NVARCHAR(MAX) NULL,
    ResolvedBy UNIQUEIDENTIFIER NULL,
    ResolvedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SettlementReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    SettlementDate DATETIME2 NOT NULL,
    SettlementPeriod INT NOT NULL, -- Daily, Weekly, Monthly
    TotalTransactions INT NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    SettledAmount DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    FailedAmount DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    Status INT NOT NULL DEFAULT 0, -- Pending, Settled, Reconciled, DiscrepancyFound
    BankSettlementRefNo NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.StudentAssignments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    AssignmentId UNIQUEIDENTIFIER NOT NULL,
    SubmissionDate DATETIME2 NULL,
    Score INT NULL,
    Feedback NVARCHAR(MAX) NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.StudentFees (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId UNIQUEIDENTIFIER NOT NULL,
    AcademicYear NVARCHAR(20) NOT NULL,
    TotalFees DECIMAL(18,4) NOT NULL,
    PaidAmount DECIMAL(18,4) NOT NULL DEFAULT 0.00,
    BalanceDue DECIMAL(18,4) NOT NULL,
    DueDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SystemNotifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    NotificationType INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    TargetUserRole INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.SystemSettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.TicketResales (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OriginalTicketId UNIQUEIDENTIFIER NOT NULL,
    OriginalBuyerId UNIQUEIDENTIFIER NOT NULL,
    NewBuyerId UNIQUEIDENTIFIER NOT NULL,
    ResalePrice DECIMAL(18,4) NOT NULL,
    PlatformFee DECIMAL(18,4) NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Listed, Sold, Cancelled
    ListedAt DATETIME2 NOT NULL,
    SoldAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.TicketReservations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SeatId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    PersonId UNIQUEIDENTIFIER NOT NULL,
    ReservationCode NVARCHAR(50) NOT NULL UNIQUE,
    ReservedPrice DECIMAL(18,4) NOT NULL,
    ReservedAt DATETIME2 NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, Expired, Completed, Cancelled
    PaymentAttempts INT NOT NULL DEFAULT 0,
    LastAttemptError NVARCHAR(MAX) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.TrainingPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Instructor NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Duration INT NOT NULL, -- in hours
    MaxParticipants INT NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- Scheduled, InProgress, Completed, Cancelled
    Location NVARCHAR(200) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.TrainingSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TrainingProgramId UNIQUEIDENTIFIER NOT NULL,
    SessionDate DATE NOT NULL,
    SessionTime TIME NOT NULL,
    Duration INT NOT NULL, -- in minutes
    Location NVARCHAR(200) NULL,
    Status INT NOT NULL DEFAULT 0, -- Scheduled, Completed, Cancelled
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.UserRoles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE(UserId, RoleId)
);

-- Access Control System
GO
CREATE TABLE dbo.VehicleRegistration (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    LicensePlate NVARCHAR(20) NOT NULL UNIQUE,
    VehicleType NVARCHAR(50) NOT NULL,
    Make NVARCHAR(100) NULL,
    Model NVARCHAR(100) NULL,
    Color NVARCHAR(50) NULL,
    RegistrationDate DATE NOT NULL,
    ExpiryDate DATE NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.VirtualPosIntegrations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProviderId UNIQUEIDENTIFIER NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL UNIQUE, -- CardRecharge, EventTicket, Parking, Cafeteria, StudentPayment, LibraryFine, Other
    Priority INT NOT NULL CHECK (Priority IN (1,2,3)), -- 1=Primary, 2=Secondary, 3=Backup
    IsDefault BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    FailoverBehavior INT NOT NULL DEFAULT 0, -- NoFailover, AllowSecondary, AllowAll
    RetryPolicy INT NOT NULL DEFAULT 0, -- NoRetry, Linear, Exponential
    MaxRetryCount INT NOT NULL DEFAULT 3,
    RetryDelayMs INT NOT NULL DEFAULT 1000,
    MinTransactionAmountOverride DECIMAL(18,4) NULL,
    MaxTransactionAmountOverride DECIMAL(18,4) NULL,
    SuccessCallbackURL NVARCHAR(500) NULL,
    FailureCallbackURL NVARCHAR(500) NULL,
    WebhookSecret NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.VirtualPosProviders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BankName NVARCHAR(100) NOT NULL UNIQUE,
    BankCode NVARCHAR(20) NOT NULL UNIQUE,
    ProviderType INT NOT NULL, -- VirtualPOS, DirectBank, EFT, Gateway
    APIEndpoint NVARCHAR(500) NOT NULL,
    APIVersion INT NOT NULL DEFAULT 1,
    SupportedPaymentMethods NVARCHAR(MAX) NOT NULL, -- JSON array
    TransactionFeeType INT NOT NULL, -- Fixed, Percentage, TieredPercentage
    TransactionFee DECIMAL(18,4) NOT NULL,
    RequiresApproval BIT NOT NULL DEFAULT 0,
    MinTransactionAmount INT NOT NULL DEFAULT 100,
    MaxTransactionAmount INT NOT NULL DEFAULT 1000000,
    IsActive BIT NOT NULL DEFAULT 1,
    ContactEmail NVARCHAR(100) NULL,
    TechnicalSupportPhone NVARCHAR(20) NULL,
    DocumentationURL NVARCHAR(500) NULL,
    Status INT NOT NULL DEFAULT 0, -- Active, InDevelopment, Deprecated, Suspended
    RowVersion TIMESTAMP NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.WalletTransactionHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonWalletTransactionId UNIQUEIDENTIFIER NOT NULL,
    PreviousStatus INT NOT NULL, -- Pending, Processing, Successful, Failed, Reversed, Cancelled
    NewStatus INT NOT NULL,
    ReasonForChange NVARCHAR(MAX) NULL,
    ChangedBy UNIQUEIDENTIFIER NOT NULL,
    ChangedAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO
CREATE TABLE dbo.WorkflowApprovals (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequestId UNIQUEIDENTIFIER NOT NULL,
    ApproverUserId UNIQUEIDENTIFIER NOT NULL,
    ApprovalStatus INT NOT NULL DEFAULT 0,
    ApprovalNotes NVARCHAR(MAX) NULL,
    ApprovedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Other
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- Get Person Wallet Balance
CREATE PROCEDURE sp_GetPersonWalletBalance
    @PersonId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        PersonId,
        ISNULL(SUM(Amount), 0) AS CurrentBalance
    FROM dbo.PersonWalletTransactions
    WHERE PersonId = @PersonId 
        AND Status = 2 -- Successful
        AND IsDeleted = 0
    GROUP BY PersonId;
END;

GO

-- Record Card Transaction
CREATE PROCEDURE sp_RecordCardTransaction
    @PersonId UNIQUEIDENTIFIER,
    @CardId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18,4),
    @TransactionType INT,
    @Description NVARCHAR(MAX),
    @ReferenceNo NVARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @CurrentBalance DECIMAL(18,4);
        DECLARE @NewBalance DECIMAL(18,4);
        
        -- Get current balance
        SELECT @CurrentBalance = ISNULL(SUM(Amount), 0)
        FROM dbo.PersonWalletTransactions
        WHERE PersonId = @PersonId 
            AND Status = 2
            AND IsDeleted = 0;
        
        -- Calculate new balance
        SET @NewBalance = @CurrentBalance + @Amount;
        
        -- Insert transaction
        INSERT INTO dbo.PersonWalletTransactions (
            PersonId, CardId, TransactionType, TransactionCategory, Amount,
            BalanceBefore, BalanceAfter, TransactionSource, Description,
            ReferenceNo, Status, PaymentMethod, CreatedAt, UpdatedAt
        )
        VALUES (
            @PersonId, @CardId, @TransactionType, 0, @Amount,
            @CurrentBalance, @NewBalance, 0, @Description,
            @ReferenceNo, 2, 0, GETUTCDATE(), GETUTCDATE()
        );
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO

-- ============================================================
-- END OF OPTIMIZED SCHEMA
-- ============================================================

GO
-- ============================================================
-- ADD FOREIGN KEY CONSTRAINTS
-- ============================================================
GO

ALTER TABLE dbo.AccessControlDevices
  ADD CONSTRAINT FK_AccessControlDevices_ChannelId
  FOREIGN KEY (ChannelId) REFERENCES dbo.AccessControlChannels(Id);
GO

ALTER TABLE dbo.AccessGroupPermissions
  ADD CONSTRAINT FK_AccessGroupPermissions_AccessGroupId
  FOREIGN KEY (AccessGroupId) REFERENCES dbo.AccessGroups(Id);
GO

ALTER TABLE dbo.AccessGroupPermissions
  ADD CONSTRAINT FK_AccessGroupPermissions_ChannelId
  FOREIGN KEY (ChannelId) REFERENCES dbo.AccessControlChannels(Id);
GO

ALTER TABLE dbo.AccessLogs
  ADD CONSTRAINT FK_AccessLogs_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.AccessLogs
  ADD CONSTRAINT FK_AccessLogs_CardId
  FOREIGN KEY (CardId) REFERENCES dbo.Cards(Id);
GO

ALTER TABLE dbo.AccessLogs
  ADD CONSTRAINT FK_AccessLogs_ManuallyCreatedByUserId
  FOREIGN KEY (ManuallyCreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.AccessLogs
  ADD CONSTRAINT FK_AccessLogs_CancelledByUserId
  FOREIGN KEY (CancelledByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.AccessScheduleExceptions
  ADD CONSTRAINT FK_AccessScheduleExceptions_AccessGroupId
  FOREIGN KEY (AccessGroupId) REFERENCES dbo.AccessGroups(Id);
GO

ALTER TABLE dbo.Addresses
  ADD CONSTRAINT FK_Addresses_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.AdvisoryNotes
  ADD CONSTRAINT FK_AdvisoryNotes_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.AdvisoryNotes
  ADD CONSTRAINT FK_AdvisoryNotes_AdvisorId
  FOREIGN KEY (AdvisorId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.Announcements
  ADD CONSTRAINT FK_Announcements_CreatedByUserId
  FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.AnomalyAlerts
  ADD CONSTRAINT FK_AnomalyAlerts_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.AnomalyAlerts
  ADD CONSTRAINT FK_AnomalyAlerts_DeviceId
  FOREIGN KEY (DeviceId) REFERENCES dbo.AccessControlDevices(Id);
GO

ALTER TABLE dbo.Attendances
  ADD CONSTRAINT FK_Attendances_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.Attendances
  ADD CONSTRAINT FK_Attendances_SessionId
  FOREIGN KEY (SessionId) REFERENCES dbo.CourseSessions(Id);
GO

ALTER TABLE dbo.AuditLogs
  ADD CONSTRAINT FK_AuditLogs_CreatedByUserId
  FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.CafeteriaAccessLogs
  ADD CONSTRAINT FK_CafeteriaAccessLogs_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.CafeteriaAccessLogs
  ADD CONSTRAINT FK_CafeteriaAccessLogs_CardId
  FOREIGN KEY (CardId) REFERENCES dbo.Cards(Id);
GO

ALTER TABLE dbo.CafeteriaAccessLogs
  ADD CONSTRAINT FK_CafeteriaAccessLogs_ManuallyCreatedByUserId
  FOREIGN KEY (ManuallyCreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.CafeteriaAccessLogs
  ADD CONSTRAINT FK_CafeteriaAccessLogs_CancelledByUserId
  FOREIGN KEY (CancelledByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.CafeteriaDailyUsage
  ADD CONSTRAINT FK_CafeteriaDailyUsage_CafeteriaId
  FOREIGN KEY (CafeteriaId) REFERENCES dbo.Cafeterias(Id);
GO

ALTER TABLE dbo.CafeteriaInventory
  ADD CONSTRAINT FK_CafeteriaInventory_CafeteriaId
  FOREIGN KEY (CafeteriaId) REFERENCES dbo.Cafeterias(Id);
GO

ALTER TABLE dbo.CafeteriaPricingRules
  ADD CONSTRAINT FK_CafeteriaPricingRules_CafeteriaId
  FOREIGN KEY (CafeteriaId) REFERENCES dbo.Cafeterias(Id);
GO

ALTER TABLE dbo.CafeteriaSubscriptions
  ADD CONSTRAINT FK_CafeteriaSubscriptions_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.Cards
  ADD CONSTRAINT FK_Cards_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.ClassroomEquipment
  ADD CONSTRAINT FK_ClassroomEquipment_ClassroomId
  FOREIGN KEY (ClassroomId) REFERENCES dbo.Classrooms(Id);
GO

ALTER TABLE dbo.ClubEvents
  ADD CONSTRAINT FK_ClubEvents_ClubId
  FOREIGN KEY (ClubId) REFERENCES dbo.StudentClubs(Id);
GO

ALTER TABLE dbo.ClubMembers
  ADD CONSTRAINT FK_ClubMembers_ClubId
  FOREIGN KEY (ClubId) REFERENCES dbo.StudentClubs(Id);
GO

ALTER TABLE dbo.ClubMembers
  ADD CONSTRAINT FK_ClubMembers_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.CorrectionApprovalRequests
  ADD CONSTRAINT FK_CorrectionApprovalRequests_ManualCorrectionLogId
  FOREIGN KEY (ManualCorrectionLogId) REFERENCES dbo.ManualCorrectionsLog(Id);
GO

ALTER TABLE dbo.CorrectionApprovalRequests
  ADD CONSTRAINT FK_CorrectionApprovalRequests_ApprovedByUserId
  FOREIGN KEY (ApprovedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.CourseAnnouncements
  ADD CONSTRAINT FK_CourseAnnouncements_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseAssignments
  ADD CONSTRAINT FK_CourseAssignments_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseCapacityConfig
  ADD CONSTRAINT FK_CourseCapacityConfig_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseEnrollments
  ADD CONSTRAINT FK_CourseEnrollments_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.CourseEnrollments
  ADD CONSTRAINT FK_CourseEnrollments_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseMaterials
  ADD CONSTRAINT FK_CourseMaterials_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseRegistrations
  ADD CONSTRAINT FK_CourseRegistrations_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.CourseRegistrations
  ADD CONSTRAINT FK_CourseRegistrations_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseSessions
  ADD CONSTRAINT FK_CourseSessions_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.CourseWaitingList
  ADD CONSTRAINT FK_CourseWaitingList_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.CourseWaitingList
  ADD CONSTRAINT FK_CourseWaitingList_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.Curricula
  ADD CONSTRAINT FK_Curricula_DepartmentId
  FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(Id);
GO

ALTER TABLE dbo.CurriculumCourses
  ADD CONSTRAINT FK_CurriculumCourses_CurriculumId
  FOREIGN KEY (CurriculumId) REFERENCES dbo.Curricula(Id);
GO

ALTER TABLE dbo.CurriculumCourses
  ADD CONSTRAINT FK_CurriculumCourses_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.Department
  ADD CONSTRAINT FK_Department_FacultyId
  FOREIGN KEY (FacultyId) REFERENCES dbo.Faculty(Id);
GO

ALTER TABLE dbo.DeviceConfig
  ADD CONSTRAINT FK_DeviceConfig_DeviceId
  FOREIGN KEY (DeviceId) REFERENCES dbo.AccessControlDevices(Id);
GO

ALTER TABLE dbo.DeviceHeartbeat
  ADD CONSTRAINT FK_DeviceHeartbeat_DeviceId
  FOREIGN KEY (DeviceId) REFERENCES dbo.AccessControlDevices(Id);
GO

ALTER TABLE dbo.DeviceOfflineBuffer
  ADD CONSTRAINT FK_DeviceOfflineBuffer_DeviceId
  FOREIGN KEY (DeviceId) REFERENCES dbo.AccessControlDevices(Id);
GO

ALTER TABLE dbo.Documents
  ADD CONSTRAINT FK_Documents_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.EmergencyContacts
  ADD CONSTRAINT FK_EmergencyContacts_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.EventCancellations
  ADD CONSTRAINT FK_EventCancellations_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.EventCheckIns
  ADD CONSTRAINT FK_EventCheckIns_EventTicketId
  FOREIGN KEY (EventTicketId) REFERENCES dbo.EventTickets(Id);
GO

ALTER TABLE dbo.EventCheckIns
  ADD CONSTRAINT FK_EventCheckIns_CheckInPersonId
  FOREIGN KEY (CheckInPersonId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.EventRefundPolicies
  ADD CONSTRAINT FK_EventRefundPolicies_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_SeatId
  FOREIGN KEY (SeatId) REFERENCES dbo.Seats(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_BuyerId
  FOREIGN KEY (BuyerId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_CardId
  FOREIGN KEY (CardId) REFERENCES dbo.Cards(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_VirtualPosProviderId
  FOREIGN KEY (VirtualPosProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.EventTickets
  ADD CONSTRAINT FK_EventTickets_PaymentTransactionLogId
  FOREIGN KEY (PaymentTransactionLogId) REFERENCES dbo.PaymentTransactionsLog(Id);
GO

ALTER TABLE dbo.Events
  ADD CONSTRAINT FK_Events_VenueId
  FOREIGN KEY (VenueId) REFERENCES dbo.Venue(Id);
GO

ALTER TABLE dbo.Events
  ADD CONSTRAINT FK_Events_OrganizerId
  FOREIGN KEY (OrganizerId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.ExamConflictLog
  ADD CONSTRAINT FK_ExamConflictLog_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.ExamConflictLog
  ADD CONSTRAINT FK_ExamConflictLog_Exam1Id
  FOREIGN KEY (Exam1Id) REFERENCES dbo.Exams(Id);
GO

ALTER TABLE dbo.ExamConflictLog
  ADD CONSTRAINT FK_ExamConflictLog_Exam2Id
  FOREIGN KEY (Exam2Id) REFERENCES dbo.Exams(Id);
GO

ALTER TABLE dbo.Exams
  ADD CONSTRAINT FK_Exams_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.Faculty
  ADD CONSTRAINT FK_Faculty_CampusId
  FOREIGN KEY (CampusId) REFERENCES dbo.Campus(Id);
GO

ALTER TABLE dbo.Fees
  ADD CONSTRAINT FK_Fees_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.FileAttachments
  ADD CONSTRAINT FK_FileAttachments_UploadedByUserId
  FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.GradeObjections
  ADD CONSTRAINT FK_GradeObjections_GradeId
  FOREIGN KEY (GradeId) REFERENCES dbo.Grades(Id);
GO

ALTER TABLE dbo.GradeObjections
  ADD CONSTRAINT FK_GradeObjections_ReviewedBy
  FOREIGN KEY (ReviewedBy) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.Grades
  ADD CONSTRAINT FK_Grades_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.Grades
  ADD CONSTRAINT FK_Grades_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.GradingScaleConfig
  ADD CONSTRAINT FK_GradingScaleConfig_DepartmentId
  FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(Id);
GO

ALTER TABLE dbo.HealthRecords
  ADD CONSTRAINT FK_HealthRecords_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.HourlyAccessSummary
  ADD CONSTRAINT FK_HourlyAccessSummary_ChannelId
  FOREIGN KEY (ChannelId) REFERENCES dbo.AccessControlChannels(Id);
GO

ALTER TABLE dbo.LaboratoryBookings
  ADD CONSTRAINT FK_LaboratoryBookings_LabId
  FOREIGN KEY (LabId) REFERENCES dbo.Laboratory(Id);
GO

ALTER TABLE dbo.LaboratoryBookings
  ADD CONSTRAINT FK_LaboratoryBookings_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.LaboratoryEquipment
  ADD CONSTRAINT FK_LaboratoryEquipment_LabId
  FOREIGN KEY (LabId) REFERENCES dbo.Laboratory(Id);
GO

ALTER TABLE dbo.LaboratoryExperiments
  ADD CONSTRAINT FK_LaboratoryExperiments_LabId
  FOREIGN KEY (LabId) REFERENCES dbo.Laboratory(Id);
GO

ALTER TABLE dbo.LibraryBorrowings
  ADD CONSTRAINT FK_LibraryBorrowings_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.LibraryBorrowings
  ADD CONSTRAINT FK_LibraryBorrowings_BookId
  FOREIGN KEY (BookId) REFERENCES dbo.LibraryBooks(Id);
GO

ALTER TABLE dbo.LibraryFines
  ADD CONSTRAINT FK_LibraryFines_LoanId
  FOREIGN KEY (LoanId) REFERENCES dbo.LibraryLoans(Id);
GO

ALTER TABLE dbo.LibraryLoans
  ADD CONSTRAINT FK_LibraryLoans_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.LibraryLoans
  ADD CONSTRAINT FK_LibraryLoans_MaterialId
  FOREIGN KEY (MaterialId) REFERENCES dbo.LibraryMaterials(Id);
GO

ALTER TABLE dbo.LibraryReservationNotifications
  ADD CONSTRAINT FK_LibraryReservationNotifications_ReservationId
  FOREIGN KEY (ReservationId) REFERENCES dbo.LibraryReservations(Id);
GO

ALTER TABLE dbo.LibraryReservations
  ADD CONSTRAINT FK_LibraryReservations_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.LibraryReservations
  ADD CONSTRAINT FK_LibraryReservations_MaterialId
  FOREIGN KEY (MaterialId) REFERENCES dbo.LibraryMaterials(Id);
GO

ALTER TABLE dbo.ManualCorrectionsLog
  ADD CONSTRAINT FK_ManualCorrectionsLog_CreatedByUserId
  FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.MazeretRaporlari
  ADD CONSTRAINT FK_MazeretRaporlari_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.MazeretRaporlari
  ADD CONSTRAINT FK_MazeretRaporlari_OnaylayanId
  FOREIGN KEY (OnaylayanId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.MealPlans
  ADD CONSTRAINT FK_MealPlans_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.MessageRecipients
  ADD CONSTRAINT FK_MessageRecipients_MessageId
  FOREIGN KEY (MessageId) REFERENCES dbo.Messages(Id);
GO

ALTER TABLE dbo.MessageRecipients
  ADD CONSTRAINT FK_MessageRecipients_RecipientId
  FOREIGN KEY (RecipientId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.Messages
  ADD CONSTRAINT FK_Messages_SenderId
  FOREIGN KEY (SenderId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.ParkingCards
  ADD CONSTRAINT FK_ParkingCards_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.ParkingEntryExitLog
  ADD CONSTRAINT FK_ParkingEntryExitLog_ParkingCardId
  FOREIGN KEY (ParkingCardId) REFERENCES dbo.ParkingCards(Id);
GO

ALTER TABLE dbo.ParkingEntryExitLog
  ADD CONSTRAINT FK_ParkingEntryExitLog_ParkingLotId
  FOREIGN KEY (ParkingLotId) REFERENCES dbo.ParkingLots(Id);
GO

ALTER TABLE dbo.ParkingEntryExitLog
  ADD CONSTRAINT FK_ParkingEntryExitLog_ManuallyCreatedByUserId
  FOREIGN KEY (ManuallyCreatedByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.ParkingEntryExitLog
  ADD CONSTRAINT FK_ParkingEntryExitLog_CancelledByUserId
  FOREIGN KEY (CancelledByUserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.ParkingRateConfig
  ADD CONSTRAINT FK_ParkingRateConfig_ParkingLotId
  FOREIGN KEY (ParkingLotId) REFERENCES dbo.ParkingLots(Id);
GO

ALTER TABLE dbo.ParkingReservation
  ADD CONSTRAINT FK_ParkingReservation_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.ParkingReservation
  ADD CONSTRAINT FK_ParkingReservation_ParkingLotId
  FOREIGN KEY (ParkingLotId) REFERENCES dbo.ParkingLots(Id);
GO

ALTER TABLE dbo.ParkingReservationUsage
  ADD CONSTRAINT FK_ParkingReservationUsage_ReservationId
  FOREIGN KEY (ReservationId) REFERENCES dbo.ParkingReservation(Id);
GO

ALTER TABLE dbo.ParkingTransactions
  ADD CONSTRAINT FK_ParkingTransactions_ParkingCardId
  FOREIGN KEY (ParkingCardId) REFERENCES dbo.ParkingCards(Id);
GO

ALTER TABLE dbo.PaySlips
  ADD CONSTRAINT FK_PaySlips_StaffId
  FOREIGN KEY (StaffId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.PaymentPlanInstallments
  ADD CONSTRAINT FK_PaymentPlanInstallments_PaymentPlanId
  FOREIGN KEY (PaymentPlanId) REFERENCES dbo.PaymentPlans(Id);
GO

ALTER TABLE dbo.PaymentPlans
  ADD CONSTRAINT FK_PaymentPlans_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.PaymentProviderCredentials
  ADD CONSTRAINT FK_PaymentProviderCredentials_ProviderId
  FOREIGN KEY (ProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.PaymentProviderCredentials
  ADD CONSTRAINT FK_PaymentProviderCredentials_CreatedBy
  FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.PaymentProviderCredentials
  ADD CONSTRAINT FK_PaymentProviderCredentials_LastUpdatedBy
  FOREIGN KEY (LastUpdatedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.PaymentReversals
  ADD CONSTRAINT FK_PaymentReversals_OriginalTransactionLogId
  FOREIGN KEY (OriginalTransactionLogId) REFERENCES dbo.PaymentTransactionsLog(Id);
GO

ALTER TABLE dbo.PaymentReversals
  ADD CONSTRAINT FK_PaymentReversals_InitiatedBy
  FOREIGN KEY (InitiatedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.PaymentTransactionsLog
  ADD CONSTRAINT FK_PaymentTransactionsLog_ProviderId
  FOREIGN KEY (ProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.PersonAccessGroups
  ADD CONSTRAINT FK_PersonAccessGroups_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.PersonAccessGroups
  ADD CONSTRAINT FK_PersonAccessGroups_AccessGroupId
  FOREIGN KEY (AccessGroupId) REFERENCES dbo.AccessGroups(Id);
GO

ALTER TABLE dbo.PersonRestrictions
  ADD CONSTRAINT FK_PersonRestrictions_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.PersonWalletTransactions
  ADD CONSTRAINT FK_PersonWalletTransactions_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.PersonWalletTransactions
  ADD CONSTRAINT FK_PersonWalletTransactions_CardId
  FOREIGN KEY (CardId) REFERENCES dbo.Cards(Id);
GO

ALTER TABLE dbo.PersonWalletTransactions
  ADD CONSTRAINT FK_PersonWalletTransactions_VirtualPosProviderId
  FOREIGN KEY (VirtualPosProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.PersonWalletTransactions
  ADD CONSTRAINT FK_PersonWalletTransactions_PaymentTransactionLogId
  FOREIGN KEY (PaymentTransactionLogId) REFERENCES dbo.PaymentTransactionsLog(Id);
GO

ALTER TABLE dbo.PersonWalletTransactions
  ADD CONSTRAINT FK_PersonWalletTransactions_ApprovedBy
  FOREIGN KEY (ApprovedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.PrerequisiteWaivers
  ADD CONSTRAINT FK_PrerequisiteWaivers_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.PrerequisiteWaivers
  ADD CONSTRAINT FK_PrerequisiteWaivers_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.PrerequisiteWaivers
  ADD CONSTRAINT FK_PrerequisiteWaivers_ApprovedBy
  FOREIGN KEY (ApprovedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.Prerequisites
  ADD CONSTRAINT FK_Prerequisites_CourseId
  FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.Prerequisites
  ADD CONSTRAINT FK_Prerequisites_PrerequisiteCourseId
  FOREIGN KEY (PrerequisiteCourseId) REFERENCES dbo.Courses(Id);
GO

ALTER TABLE dbo.Refunds
  ADD CONSTRAINT FK_Refunds_OriginalTransactionId
  FOREIGN KEY (OriginalTransactionId) REFERENCES dbo.PersonWalletTransactions(Id);
GO

ALTER TABLE dbo.Refunds
  ADD CONSTRAINT FK_Refunds_RequestedBy
  FOREIGN KEY (RequestedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.Refunds
  ADD CONSTRAINT FK_Refunds_ApprovedBy
  FOREIGN KEY (ApprovedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.ResaleRestrictions
  ADD CONSTRAINT FK_ResaleRestrictions_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.ResearchProjectMembers
  ADD CONSTRAINT FK_ResearchProjectMembers_ProjectId
  FOREIGN KEY (ProjectId) REFERENCES dbo.ResearchProjects(Id);
GO

ALTER TABLE dbo.ResearchProjectMembers
  ADD CONSTRAINT FK_ResearchProjectMembers_StaffId
  FOREIGN KEY (StaffId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.ResearchProjects
  ADD CONSTRAINT FK_ResearchProjects_LeaderStaffId
  FOREIGN KEY (LeaderStaffId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.ResearchPublications
  ADD CONSTRAINT FK_ResearchPublications_StaffId
  FOREIGN KEY (StaffId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.ResearchPublications
  ADD CONSTRAINT FK_ResearchPublications_ProjectId
  FOREIGN KEY (ProjectId) REFERENCES dbo.ResearchProjects(Id);
GO

ALTER TABLE dbo.RolePermissions
  ADD CONSTRAINT FK_RolePermissions_RoleId
  FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id);
GO

ALTER TABLE dbo.RolePermissions
  ADD CONSTRAINT FK_RolePermissions_PermissionId
  FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(Id);
GO

ALTER TABLE dbo.ScholarshipDeductions
  ADD CONSTRAINT FK_ScholarshipDeductions_ScholarshipId
  FOREIGN KEY (ScholarshipId) REFERENCES dbo.Scholarships(Id);
GO

ALTER TABLE dbo.Scholarships
  ADD CONSTRAINT FK_Scholarships_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.SeatReservations
  ADD CONSTRAINT FK_SeatReservations_SeatId
  FOREIGN KEY (SeatId) REFERENCES dbo.Seats(Id);
GO

ALTER TABLE dbo.SeatReservations
  ADD CONSTRAINT FK_SeatReservations_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.SeatReservations
  ADD CONSTRAINT FK_SeatReservations_ReservedBy
  FOREIGN KEY (ReservedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.SeatingArrangement
  ADD CONSTRAINT FK_SeatingArrangement_VenueId
  FOREIGN KEY (VenueId) REFERENCES dbo.Venue(Id);
GO

ALTER TABLE dbo.SeatingArrangement
  ADD CONSTRAINT FK_SeatingArrangement_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.Seats
  ADD CONSTRAINT FK_Seats_SeatingArrangementId
  FOREIGN KEY (SeatingArrangementId) REFERENCES dbo.SeatingArrangement(Id);
GO

ALTER TABLE dbo.SettlementDiscrepancies
  ADD CONSTRAINT FK_SettlementDiscrepancies_SettlementReportId
  FOREIGN KEY (SettlementReportId) REFERENCES dbo.SettlementReports(Id);
GO

ALTER TABLE dbo.SettlementDiscrepancies
  ADD CONSTRAINT FK_SettlementDiscrepancies_OriginalTransactionLogId
  FOREIGN KEY (OriginalTransactionLogId) REFERENCES dbo.PaymentTransactionsLog(Id);
GO

ALTER TABLE dbo.SettlementDiscrepancies
  ADD CONSTRAINT FK_SettlementDiscrepancies_ResolvedBy
  FOREIGN KEY (ResolvedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.SettlementReports
  ADD CONSTRAINT FK_SettlementReports_ProviderId
  FOREIGN KEY (ProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.Staff
  ADD CONSTRAINT FK_Staff_UserId
  FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.Staff
  ADD CONSTRAINT FK_Staff_DepartmentId
  FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(Id);
GO

ALTER TABLE dbo.StudentAssignments
  ADD CONSTRAINT FK_StudentAssignments_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.StudentAssignments
  ADD CONSTRAINT FK_StudentAssignments_AssignmentId
  FOREIGN KEY (AssignmentId) REFERENCES dbo.CourseAssignments(Id);
GO

ALTER TABLE dbo.StudentClubs
  ADD CONSTRAINT FK_StudentClubs_PresidentId
  FOREIGN KEY (PresidentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.StudentFees
  ADD CONSTRAINT FK_StudentFees_StudentId
  FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id);
GO

ALTER TABLE dbo.Students
  ADD CONSTRAINT FK_Students_CampusId
  FOREIGN KEY (CampusId) REFERENCES dbo.Campus(Id);
GO

ALTER TABLE dbo.Students
  ADD CONSTRAINT FK_Students_FacultyId
  FOREIGN KEY (FacultyId) REFERENCES dbo.Faculty(Id);
GO

ALTER TABLE dbo.Students
  ADD CONSTRAINT FK_Students_DepartmentId
  FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(Id);
GO

ALTER TABLE dbo.Students
  ADD CONSTRAINT FK_Students_AdvisorStaffId
  FOREIGN KEY (AdvisorStaffId) REFERENCES dbo.Staff(Id);
GO

ALTER TABLE dbo.TicketResales
  ADD CONSTRAINT FK_TicketResales_OriginalTicketId
  FOREIGN KEY (OriginalTicketId) REFERENCES dbo.EventTickets(Id);
GO

ALTER TABLE dbo.TicketResales
  ADD CONSTRAINT FK_TicketResales_OriginalBuyerId
  FOREIGN KEY (OriginalBuyerId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.TicketResales
  ADD CONSTRAINT FK_TicketResales_NewBuyerId
  FOREIGN KEY (NewBuyerId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.TicketReservations
  ADD CONSTRAINT FK_TicketReservations_SeatId
  FOREIGN KEY (SeatId) REFERENCES dbo.Seats(Id);
GO

ALTER TABLE dbo.TicketReservations
  ADD CONSTRAINT FK_TicketReservations_EventId
  FOREIGN KEY (EventId) REFERENCES dbo.Events(Id);
GO

ALTER TABLE dbo.TicketReservations
  ADD CONSTRAINT FK_TicketReservations_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.TrainingSessions
  ADD CONSTRAINT FK_TrainingSessions_TrainingProgramId
  FOREIGN KEY (TrainingProgramId) REFERENCES dbo.TrainingPrograms(Id);
GO

ALTER TABLE dbo.UserRoles
  ADD CONSTRAINT FK_UserRoles_UserId
  FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.UserRoles
  ADD CONSTRAINT FK_UserRoles_RoleId
  FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id);
GO

ALTER TABLE dbo.VehicleRegistration
  ADD CONSTRAINT FK_VehicleRegistration_PersonId
  FOREIGN KEY (PersonId) REFERENCES dbo.Persons(Id);
GO

ALTER TABLE dbo.VirtualPosIntegrations
  ADD CONSTRAINT FK_VirtualPosIntegrations_ProviderId
  FOREIGN KEY (ProviderId) REFERENCES dbo.VirtualPosProviders(Id);
GO

ALTER TABLE dbo.WalletTransactionHistory
  ADD CONSTRAINT FK_WalletTransactionHistory_PersonWalletTransactionId
  FOREIGN KEY (PersonWalletTransactionId) REFERENCES dbo.PersonWalletTransactions(Id);
GO

ALTER TABLE dbo.WalletTransactionHistory
  ADD CONSTRAINT FK_WalletTransactionHistory_ChangedBy
  FOREIGN KEY (ChangedBy) REFERENCES dbo.Users(Id);
GO

ALTER TABLE dbo.WorkflowApprovals
  ADD CONSTRAINT FK_WorkflowApprovals_ApproverUserId
  FOREIGN KEY (ApproverUserId) REFERENCES dbo.Users(Id);
GO