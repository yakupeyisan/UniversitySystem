-- ============================================================
-- KAPSAMLI ÜNİVERSİTE YÖNETİM SİSTEMİ
-- COMPLETE DATABASE SCHEMA - VERSION 3.0
-- ============================================================
-- Version: 3.0 - FULLY COMPREHENSIVE WITH ALL MODULES
-- Date: 31 Ekim 2025
-- Description: Complete schema for university management system
--              Including ALL modules: Academic, HR, Payroll, Finance, 
--              Procurement, Inventory, Library, Security, Parking, 
--              Events, Health, Laboratory, Research, IT, Facilities, etc.
-- Database: SQL Server 2022+
-- Total Modules: 18 Main Modules, 100+ Sub-systems
-- Total Entities: 300+ Tables
-- ============================================================

USE
[master];
GO

-- ============================================================
-- CLEAN START: Drop Everything
-- ============================================================
PRINT '🧹 Starting clean installation...';

-- 1️⃣ Drop all foreign keys
PRINT
'Dropping all foreign keys...';
DECLARE
@sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' +
               OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];'
FROM sys.foreign_keys;
EXEC sp_executesql @sql;

-- 2️⃣ Drop all stored procedures
PRINT
'Dropping all stored procedures...';
DECLARE
@procSql NVARCHAR(MAX) = '';
SELECT @procSql += 'DROP PROCEDURE [' + SCHEMA_NAME(schema_id) + '].[' + name + '];' + CHAR(13)
FROM sys.procedures;
EXEC sp_executesql @procSql;

-- 3️⃣ Drop all tables
PRINT
'Dropping all tables...';
EXEC sp_MSforeachtable 'DROP TABLE ?';

PRINT
'✅ All existing objects dropped successfully.';
GO
-- Create Database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UniversitySystem')
BEGIN
    CREATE
DATABASE [UniversitySystem];
    PRINT
'✅ Database UniversitySystem created successfully.';
END
GO

USE [UniversitySystem];
GO

PRINT '📊 Starting table creation for University Management System v3.0...';
GO

-- ============================================================
-- MODULE 1: CORE INFRASTRUCTURE (TEMEL ALTYAPI)
-- ============================================================
PRINT '📦 Module 1: Creating Core Infrastructure tables...';
GO

-- 1.1 Persons (Base Entity)
CREATE TABLE Persons
(
    Id                       UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonType               INT  NOT NULL,                          -- 0=Student, 1=Staff, 2=Both
    IdentityNumber           NVARCHAR(20) NOT NULL UNIQUE,
    FirstName                NVARCHAR(100) NOT NULL,
    LastName                 NVARCHAR(100) NOT NULL,
    Gender                   INT  NOT NULL,                          -- 0=Male, 1=Female, 2=Other
    DateOfBirth              DATE NOT NULL,
    Nationality              NVARCHAR(100) NOT NULL DEFAULT 'Turkish',
    Email                    NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber              NVARCHAR(20) NULL,
    ProfilePicturePath       NVARCHAR(500) NULL,
    BloodType                NVARCHAR(5) NULL,
    MaritalStatus            INT                          DEFAULT 0, -- 0=Single, 1=Married, 2=Divorced, 3=Widowed
    EmergencyContactName     NVARCHAR(100) NULL,
    EmergencyContactPhone    NVARCHAR(20) NULL,
    EmergencyContactRelation NVARCHAR(50) NULL,
    Status                   INT                          DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended, 3=Terminated
    IsDeleted                BIT                          DEFAULT 0,
    CreatedAt                DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt                DATETIME2                    DEFAULT GETUTCDATE()
);

-- 1.2 Addresses
CREATE TABLE Addresses
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId     UNIQUEIDENTIFIER NOT NULL,
    AddressType  INT              NOT NULL, -- 0=Home, 1=Work, 2=Mailing
    AddressLine1 NVARCHAR(500) NOT NULL,
    AddressLine2 NVARCHAR(500) NULL,
    City         NVARCHAR(100) NOT NULL,
    District     NVARCHAR(100) NULL,
    PostalCode   NVARCHAR(20) NULL,
    Country      NVARCHAR(100) NOT NULL DEFAULT 'Turkey',
    IsPrimary    BIT                          DEFAULT 0,
    IsDeleted    BIT                          DEFAULT 0,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 1.3 Roles
CREATE TABLE Roles
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleName     NVARCHAR(100) NOT NULL UNIQUE,
    RoleCode     NVARCHAR(50) NOT NULL UNIQUE,
    Description  NVARCHAR(500) NULL,
    RoleLevel    INT                          DEFAULT 0,
    IsSystemRole BIT                          DEFAULT 0,
    IsActive     BIT                          DEFAULT 1,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE()
);

-- 1.4 UserRoles (Many-to-Many)
CREATE TABLE UserRoles
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId     UNIQUEIDENTIFIER NOT NULL,
    RoleId       UNIQUEIDENTIFIER NOT NULL,
    AssignedDate DATETIME2                    DEFAULT GETUTCDATE(),
    AssignedBy   UNIQUEIDENTIFIER NULL,
    IsActive     BIT                          DEFAULT 1,
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (RoleId) REFERENCES Roles (Id)
);

-- 1.5 System Settings
CREATE TABLE SystemSettings
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SettingKey     NVARCHAR(200) NOT NULL UNIQUE,
    SettingValue   NVARCHAR(MAX) NOT NULL,
    SettingType    NVARCHAR(50) NOT NULL,
    Category       NVARCHAR(100) NOT NULL,
    Description    NVARCHAR(500) NULL,
    IsEditable     BIT                          DEFAULT 1,
    LastModifiedBy UNIQUEIDENTIFIER NULL,
    LastModifiedAt DATETIME2                    DEFAULT GETUTCDATE()
);

-- 1.6 System Notifications
CREATE TABLE SystemNotifications
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    NotificationType INT NOT NULL, -- 0=Info, 1=Warning, 2=Error, 3=Success
    Title            NVARCHAR(200) NOT NULL,
    Message          NVARCHAR(MAX) NOT NULL,
    TargetPersonId   UNIQUEIDENTIFIER NULL,
    TargetRole       NVARCHAR(100) NULL,
    IsRead           BIT                          DEFAULT 0,
    ReadAt           DATETIME2 NULL,
    Priority         INT                          DEFAULT 1,
    ExpiryDate       DATETIME2 NULL,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (TargetPersonId) REFERENCES Persons (Id)
);

-- 1.7 Audit Logs
CREATE TABLE AuditLogs
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId     UNIQUEIDENTIFIER NULL,
    Action     NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId   NVARCHAR(50) NULL,
    OldValues  NVARCHAR(MAX) NULL,
    NewValues  NVARCHAR(MAX) NULL,
    IpAddress  NVARCHAR(50) NULL,
    UserAgent  NVARCHAR(500) NULL,
    Timestamp  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Persons (Id)
);

-- 1.8 Documents & Attachments
CREATE TABLE Documents
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DocumentNumber NVARCHAR(100) NOT NULL UNIQUE,
    DocumentType   NVARCHAR(100) NOT NULL,
    Title          NVARCHAR(300) NOT NULL,
    Description    NVARCHAR(MAX) NULL,
    FilePath       NVARCHAR(500) NULL,
    FileURL        NVARCHAR(500) NULL,
    FileSize       BIGINT NULL,
    MimeType       NVARCHAR(100) NULL,
    UploadedBy     UNIQUEIDENTIFIER NOT NULL,
    UploadDate     DATETIME2                    DEFAULT GETUTCDATE(),
    IsPublic       BIT                          DEFAULT 0,
    Status         INT                          DEFAULT 0,
    FOREIGN KEY (UploadedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 1: Core Infrastructure tables created.';
GO

-- ============================================================
-- MODULE 2: ACADEMIC ORGANIZATION (AKADEMİK YAPI)
-- ============================================================
PRINT '📦 Module 2: Creating Academic Organization tables...';
GO

-- 2.1 Faculties
CREATE TABLE Faculties
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FacultyCode     NVARCHAR(20) NOT NULL UNIQUE,
    FacultyName     NVARCHAR(200) NOT NULL,
    EstablishedDate DATE NOT NULL,
    DeanId          UNIQUEIDENTIFIER NULL,
    BuildingName    NVARCHAR(100) NULL,
    PhoneNumber     NVARCHAR(20) NULL,
    Email           NVARCHAR(100) NULL,
    Website         NVARCHAR(200) NULL,
    Status          INT                          DEFAULT 0,
    IsDeleted       BIT                          DEFAULT 0,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DeanId) REFERENCES Persons (Id)
);

-- 2.2 Departments
CREATE TABLE Departments
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DepartmentCode     NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentName     NVARCHAR(200) NOT NULL,
    FacultyId          UNIQUEIDENTIFIER NOT NULL,
    HeadOfDepartmentId UNIQUEIDENTIFIER NULL,
    EstablishedDate    DATE             NOT NULL,
    BuildingName       NVARCHAR(100) NULL,
    RoomNumber         NVARCHAR(50) NULL,
    PhoneNumber        NVARCHAR(20) NULL,
    Email              NVARCHAR(100) NULL,
    Status             INT                          DEFAULT 0,
    IsDeleted          BIT                          DEFAULT 0,
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (FacultyId) REFERENCES Faculties (Id),
    FOREIGN KEY (HeadOfDepartmentId) REFERENCES Persons (Id)
);

-- 2.3 Programs (Lisans, Yüksek Lisans, Doktora)
CREATE TABLE Programs
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramCode           NVARCHAR(20) NOT NULL UNIQUE,
    ProgramName           NVARCHAR(200) NOT NULL,
    DepartmentId          UNIQUEIDENTIFIER NOT NULL,
    DegreeLevel           INT              NOT NULL, -- 0=Lisans, 1=YüksekLisans, 2=Doktora
    DurationYears         INT              NOT NULL,
    TotalCreditsRequired  INT              NOT NULL,
    LanguageOfInstruction NVARCHAR(50) DEFAULT 'Turkish',
    Status                INT                          DEFAULT 0,
    IsDeleted             BIT                          DEFAULT 0,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id)
);

-- 2.4 Semesters
CREATE TABLE Semesters
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SemesterCode NVARCHAR(20) NOT NULL UNIQUE,
    SemesterName NVARCHAR(100) NOT NULL,
    AcademicYear NVARCHAR(20) NOT NULL,
    SemesterType INT  NOT NULL, -- 0=Fall, 1=Spring, 2=Summer
    StartDate    DATE NOT NULL,
    EndDate      DATE NOT NULL,
    IsCurrent    BIT                          DEFAULT 0,
    IsActive     BIT                          DEFAULT 1,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE()
);

-- 2.5 Courses
CREATE TABLE Courses
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseCode            NVARCHAR(20) NOT NULL UNIQUE,
    CourseName            NVARCHAR(200) NOT NULL,
    DepartmentId          UNIQUEIDENTIFIER NOT NULL,
    TheoreticalHours      INT                          DEFAULT 0,
    PracticalHours        INT                          DEFAULT 0,
    LaboratoryHours       INT                          DEFAULT 0,
    TotalHours            INT              NOT NULL,
    ECTS                  INT              NOT NULL,
    LocalCredits          INT              NOT NULL,
    CourseType            INT              NOT NULL, -- 0=Compulsory, 1=Elective
    LanguageOfInstruction NVARCHAR(50) DEFAULT 'Turkish',
    CourseLevel           INT                          DEFAULT 0,
    Description           NVARCHAR(MAX) NULL,
    Objectives            NVARCHAR(MAX) NULL,
    LearningOutcomes      NVARCHAR(MAX) NULL,
    IsActive              BIT                          DEFAULT 1,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id)
);

-- 2.6 Course Prerequisites
CREATE TABLE Prerequisites
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId             UNIQUEIDENTIFIER NOT NULL,
    PrerequisiteCourseId UNIQUEIDENTIFIER NOT NULL,
    MinimumGrade         NVARCHAR(2) NULL, -- DD, DC, CC, etc.
    IsStrict             BIT                          DEFAULT 1,
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses (Id),
    FOREIGN KEY (PrerequisiteCourseId) REFERENCES Courses (Id)
);

-- 2.7 Course Schedules (Ders Programı)
CREATE TABLE CourseSchedules
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId          UNIQUEIDENTIFIER NOT NULL,
    SemesterId        UNIQUEIDENTIFIER NOT NULL,
    DayOfWeek         INT              NOT NULL, -- 1=Monday, 7=Sunday
    StartTime         TIME             NOT NULL,
    EndTime           TIME             NOT NULL,
    RoomId            UNIQUEIDENTIFIER NULL,
    InstructorId      UNIQUEIDENTIFIER NULL,
    MaxCapacity       INT                          DEFAULT 0,
    CurrentEnrollment INT                          DEFAULT 0,
    Status            INT                          DEFAULT 0,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses (Id),
    FOREIGN KEY (SemesterId) REFERENCES Semesters (Id),
    FOREIGN KEY (InstructorId) REFERENCES Persons (Id)
);

-- 2.8 Course Syllabus (Haftalık Ders Konuları)
CREATE TABLE CourseSyllabus
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId          UNIQUEIDENTIFIER NOT NULL,
    WeekNumber        INT              NOT NULL,
    TopicTitle        NVARCHAR(300) NOT NULL,
    TopicDescription  NVARCHAR(MAX) NULL,
    LearningOutcomes  NVARCHAR(MAX) NULL,
    ReadingMaterials  NVARCHAR(MAX) NULL,
    AssignmentDetails NVARCHAR(MAX) NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses (Id)
);

-- 2.9 Course Materials
CREATE TABLE CourseMaterials
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId          UNIQUEIDENTIFIER NOT NULL,
    MaterialName      NVARCHAR(200) NOT NULL,
    MaterialType      NVARCHAR(50) NOT NULL, -- PDF, PPT, Video, Code, Doc
    Description       NVARCHAR(MAX) NULL,
    FilePath          NVARCHAR(500) NULL,
    FileURL           NVARCHAR(500) NULL,
    FileSize          BIGINT NULL,
    WeekNumber        INT NULL,
    UploadDate        DATETIME2                    DEFAULT GETUTCDATE(),
    UploadedByStaffId UNIQUEIDENTIFIER NOT NULL,
    DownloadCount     INT                          DEFAULT 0,
    IsActive          BIT                          DEFAULT 1,
    FOREIGN KEY (CourseId) REFERENCES Courses (Id),
    FOREIGN KEY (UploadedByStaffId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 2: Academic Organization tables created.';
GO

-- ============================================================
-- MODULE 3: STUDENT MANAGEMENT (ÖĞRENCİ YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 3: Creating Student Management tables...';
GO

-- 3.1 Students
CREATE TABLE Students
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId              UNIQUEIDENTIFIER NOT NULL,
    StudentNumber         NVARCHAR(20) NOT NULL UNIQUE,
    ProgramId             UNIQUEIDENTIFIER NOT NULL,
    AdmissionYear         INT              NOT NULL,
    AdmissionType         INT              NOT NULL,              -- 0=Regular, 1=Lateral, 2=Transfer
    CurrentSemester       INT                          DEFAULT 1,
    AdvisorId             UNIQUEIDENTIFIER NULL,
    GANO                  DECIMAL(5, 2)                DEFAULT 0.00,
    YANO                  DECIMAL(5, 2)                DEFAULT 0.00,
    TotalCreditsCompleted INT                          DEFAULT 0,
    RegistrationStatus    INT                          DEFAULT 0, -- 0=Active, 1=Frozen, 2=Graduated, 3=Dropout
    GraduationDate        DATE NULL,
    IsDeleted             BIT                          DEFAULT 0,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (ProgramId) REFERENCES Programs (Id),
    FOREIGN KEY (AdvisorId) REFERENCES Persons (Id)
);

-- 3.2 Course Registrations
CREATE TABLE CourseRegistrations
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId             UNIQUEIDENTIFIER NOT NULL,
    CourseScheduleId      UNIQUEIDENTIFIER NOT NULL,
    SemesterId            UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate      DATETIME2                    DEFAULT GETUTCDATE(),
    Status                INT                          DEFAULT 0, -- 0=Active, 1=Dropped, 2=Completed, 3=Failed
    AdvisorApprovalStatus INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected
    AdvisorApprovalDate   DATETIME2 NULL,
    AdvisorComments       NVARCHAR(MAX) NULL,
    IsDeleted             BIT                          DEFAULT 0,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (CourseScheduleId) REFERENCES CourseSchedules (Id),
    FOREIGN KEY (SemesterId) REFERENCES Semesters (Id)
);

-- 3.3 Attendance (Devamsızlık Takibi)
CREATE TABLE Attendance
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseRegistrationId UNIQUEIDENTIFIER NOT NULL,
    StudentId            UNIQUEIDENTIFIER NOT NULL,
    AttendanceDate       DATE             NOT NULL,
    SessionNumber        INT              NOT NULL,
    Status               INT              NOT NULL, -- 0=Present, 1=Absent, 2=Excused, 3=Late
    WeekNumber           INT NULL,
    Notes                NVARCHAR(MAX) NULL,
    RecordedBy           UNIQUEIDENTIFIER NOT NULL,
    RecordedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseRegistrationId) REFERENCES CourseRegistrations (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (RecordedBy) REFERENCES Persons (Id)
);

-- 3.4 Mazeret Raporları (Excuse Reports)
CREATE TABLE ExcuseReports
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId    UNIQUEIDENTIFIER NOT NULL,
    ReportDate   DATE             NOT NULL,
    ReportType   INT              NOT NULL,              -- 0=Health, 1=Family, 2=Other
    StartDate    DATE             NOT NULL,
    EndDate      DATE             NOT NULL,
    Reason       NVARCHAR(MAX) NOT NULL,
    DocumentPath NVARCHAR(500) NULL,
    Status       INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected
    ApprovedBy   UNIQUEIDENTIFIER NULL,
    ApprovalDate DATETIME2 NULL,
    Comments     NVARCHAR(MAX) NULL,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 3: Student Management tables created.';
GO

-- ============================================================
-- MODULE 4: EXAM & GRADING (SINAV VE NOTLANDIRMA)
-- ============================================================
PRINT '📦 Module 4: Creating Exam & Grading tables...';
GO

-- 4.1 Exams
CREATE TABLE Exams
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseScheduleId UNIQUEIDENTIFIER NOT NULL,
    ExamType         INT              NOT NULL,              -- 0=Midterm, 1=Final, 2=Quiz, 3=Makeup, 4=Project
    ExamDate         DATETIME2        NOT NULL,
    Duration         INT              NOT NULL,              -- in minutes
    TotalPoints      DECIMAL(5, 2)                DEFAULT 100.00,
    PassingGrade     DECIMAL(5, 2)                DEFAULT 50.00,
    WeightPercentage DECIMAL(5, 2)    NOT NULL,
    ExamScope        NVARCHAR(MAX) NULL,
    Instructions     NVARCHAR(MAX) NULL,
    Status           INT                          DEFAULT 0, -- 0=Scheduled, 1=InProgress, 2=Completed, 3=Cancelled
    CreatedBy        UNIQUEIDENTIFIER NOT NULL,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseScheduleId) REFERENCES CourseSchedules (Id),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id)
);

-- 4.2 Exam Rooms
CREATE TABLE ExamRooms
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ExamId               UNIQUEIDENTIFIER NOT NULL,
    RoomId               UNIQUEIDENTIFIER NULL,
    RoomName             NVARCHAR(100) NOT NULL,
    Capacity             INT              NOT NULL,
    AssignedStudentCount INT                          DEFAULT 0,
    ProctorId            UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (ExamId) REFERENCES Exams (Id),
    FOREIGN KEY (ProctorId) REFERENCES Persons (Id)
);

-- 4.3 Exam Seating Arrangement
CREATE TABLE ExamSeatingArrangement
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ExamId     UNIQUEIDENTIFIER NOT NULL,
    StudentId  UNIQUEIDENTIFIER NOT NULL,
    ExamRoomId UNIQUEIDENTIFIER NOT NULL,
    SeatNumber NVARCHAR(20) NOT NULL,
    CreatedAt  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ExamId) REFERENCES Exams (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ExamRoomId) REFERENCES ExamRooms (Id)
);

-- 4.4 Grades
CREATE TABLE Grades
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseRegistrationId UNIQUEIDENTIFIER NOT NULL,
    StudentId            UNIQUEIDENTIFIER NOT NULL,
    ExamId               UNIQUEIDENTIFIER NULL,
    NumericGrade         DECIMAL(5, 2) NULL,
    LetterGrade          NVARCHAR(2) NULL,          -- AA, BA, BB, CB, CC, DC, DD, FD, FF
    GradePoints          DECIMAL(5, 2) NULL,
    GradeType            INT              NOT NULL, -- 0=Midterm, 1=Final, 2=Overall
    IsPassed             BIT                          DEFAULT 0,
    EnteredBy            UNIQUEIDENTIFIER NOT NULL,
    EnteredAt            DATETIME2                    DEFAULT GETUTCDATE(),
    IsFinalized          BIT                          DEFAULT 0,
    FinalizedAt          DATETIME2 NULL,
    FOREIGN KEY (CourseRegistrationId) REFERENCES CourseRegistrations (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ExamId) REFERENCES Exams (Id),
    FOREIGN KEY (EnteredBy) REFERENCES Persons (Id)
);

-- 4.5 Grade Appeals (Not İtiraz)
CREATE TABLE GradeAppeals
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GradeId           UNIQUEIDENTIFIER NOT NULL,
    StudentId         UNIQUEIDENTIFIER NOT NULL,
    AppealReason      NVARCHAR(MAX) NOT NULL,
    AppealDate        DATETIME2                    DEFAULT GETUTCDATE(),
    Status            INT                          DEFAULT 0, -- 0=Pending, 1=UnderReview, 2=Approved, 3=Rejected
    ReviewedByStaffId UNIQUEIDENTIFIER NULL,
    ReviewDate        DATETIME2 NULL,
    ReviewComments    NVARCHAR(MAX) NULL,
    OldGrade          NVARCHAR(2) NULL,
    NewGrade          NVARCHAR(2) NULL,
    FOREIGN KEY (GradeId) REFERENCES Grades (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ReviewedByStaffId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 4: Exam & Grading tables created.';
GO

-- ============================================================
-- MODULE 5: STAFF MANAGEMENT (PERSONEL YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 5: Creating Staff Management tables...';
GO

-- 5.1 Staff
CREATE TABLE Staff
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId        UNIQUEIDENTIFIER NOT NULL,
    StaffNumber     NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentId    UNIQUEIDENTIFIER NOT NULL,
    StaffType       INT              NOT NULL,              -- 0=Academic, 1=Administrative
    AcademicTitle   INT NULL,                               -- 0=Professor, 1=AssociateProfessor, 2=AssistantProfessor, 3=Lecturer, 4=ResearchAssistant
    HireDate        DATE             NOT NULL,
    PositionTitle   NVARCHAR(200) NOT NULL,
    OfficeLocation  NVARCHAR(200) NULL,
    OfficePhone     NVARCHAR(20) NULL,
    WeeklyWorkHours INT                          DEFAULT 40,
    Status          INT                          DEFAULT 0, -- 0=Active, 1=OnLeave, 2=Retired, 3=Terminated
    IsDeleted       BIT                          DEFAULT 0,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id)
);

-- 5.2 Employees (For HR purposes)
CREATE TABLE Employees
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId        UNIQUEIDENTIFIER NOT NULL,
    EmployeeNumber  NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentId    UNIQUEIDENTIFIER NOT NULL,
    PositionTitle   NVARCHAR(200) NOT NULL,
    EmploymentType  INT              NOT NULL, -- 0=FullTime, 1=PartTime, 2=Contract, 3=Temporary
    HireDate        DATE             NOT NULL,
    TerminationDate DATE NULL,
    ReportingTo     UNIQUEIDENTIFIER NULL,
    SalaryGrade     NVARCHAR(20) NULL,
    BaseSalary      DECIMAL(18, 4) NULL,
    Status          INT                          DEFAULT 0,
    IsDeleted       BIT                          DEFAULT 0,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (ReportingTo) REFERENCES Persons (Id)
);

PRINT
'✅ Module 5: Staff Management tables created.';
GO

-- ============================================================
-- MODULE 6: LEAVE MANAGEMENT (İZİN YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 6: Creating Leave Management tables...';
GO

-- 6.1 Leave Types
CREATE TABLE LeaveTypes
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LeaveTypeName    NVARCHAR(100) NOT NULL UNIQUE,
    LeaveCode        NVARCHAR(20) NOT NULL UNIQUE,
    Description      NVARCHAR(500) NULL,
    MaxDaysPerYear   INT NULL,
    RequiresApproval BIT                          DEFAULT 1,
    IsPaid           BIT                          DEFAULT 1,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE()
);

-- 6.2 Leave Balance
CREATE TABLE LeaveBalance
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId       UNIQUEIDENTIFIER NOT NULL,
    LeaveTypeId      UNIQUEIDENTIFIER NOT NULL,
    Year             INT              NOT NULL,
    TotalDays        INT              NOT NULL,
    UsedDays         INT                          DEFAULT 0,
    RemainingDays    INT              NOT NULL,
    CarryForwardDays INT                          DEFAULT 0,
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes (Id)
);

-- 6.3 Leave Requests
CREATE TABLE LeaveRequests
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId       UNIQUEIDENTIFIER NOT NULL,
    LeaveTypeId      UNIQUEIDENTIFIER NOT NULL,
    StartDate        DATE             NOT NULL,
    EndDate          DATE             NOT NULL,
    TotalDays        INT              NOT NULL,
    Reason           NVARCHAR(MAX) NULL,
    RequestDate      DATETIME2                    DEFAULT GETUTCDATE(),
    Status           INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected, 3=Cancelled
    ApprovedBy       UNIQUEIDENTIFIER NULL,
    ApprovalDate     DATETIME2 NULL,
    ApprovalComments NVARCHAR(MAX) NULL,
    DocumentPath     NVARCHAR(500) NULL,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    FOREIGN KEY (LeaveTypeId) REFERENCES LeaveTypes (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 6: Leave Management tables created.';
GO

-- ============================================================
-- MODULE 7: PAYROLL (BORDRO SİSTEMİ)
-- ============================================================
PRINT '📦 Module 7: Creating Payroll tables...';
GO

-- 7.1 Salary Structure
CREATE TABLE SalaryStructure
(
    Id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId              UNIQUEIDENTIFIER NOT NULL,
    BaseSalary              DECIMAL(18, 4)   NOT NULL,
    HousingAllowance        DECIMAL(18, 4)               DEFAULT 0,
    TransportationAllowance DECIMAL(18, 4)               DEFAULT 0,
    MealAllowance           DECIMAL(18, 4)               DEFAULT 0,
    OtherAllowances         DECIMAL(18, 4)               DEFAULT 0,
    AcademicIncentive       DECIMAL(18, 4)               DEFAULT 0,
    EffectiveDate           DATE             NOT NULL,
    EndDate                 DATE NULL,
    Status                  INT                          DEFAULT 0,
    CreatedAt               DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id)
);

-- 7.2 Payroll Runs
CREATE TABLE PayrollRuns
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PayrollMonth    INT              NOT NULL,
    PayrollYear     INT              NOT NULL,
    PayrollPeriod   NVARCHAR(20) NOT NULL,
    ProcessedDate   DATETIME2                    DEFAULT GETUTCDATE(),
    ProcessedBy     UNIQUEIDENTIFIER NOT NULL,
    TotalGrossPay   DECIMAL(18, 4)   NOT NULL,
    TotalDeductions DECIMAL(18, 4)   NOT NULL,
    TotalNetPay     DECIMAL(18, 4)   NOT NULL,
    Status          INT                          DEFAULT 0, -- 0=Draft, 1=Processed, 2=Approved, 3=Paid
    ApprovedBy      UNIQUEIDENTIFIER NULL,
    ApprovalDate    DATETIME2 NULL,
    FOREIGN KEY (ProcessedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 7.3 Payroll Details
CREATE TABLE PayrollDetails
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PayrollRunId          UNIQUEIDENTIFIER NOT NULL,
    EmployeeId            UNIQUEIDENTIFIER NOT NULL,
    GrossSalary           DECIMAL(18, 4)   NOT NULL,
    Allowances            DECIMAL(18, 4)               DEFAULT 0,
    OvertimePay           DECIMAL(18, 4)               DEFAULT 0,
    Bonus                 DECIMAL(18, 4)               DEFAULT 0,
    TotalGross            DECIMAL(18, 4)   NOT NULL,
    IncomeTax             DECIMAL(18, 4)               DEFAULT 0,
    SSKEmployee           DECIMAL(18, 4)               DEFAULT 0,
    SSKEmployer           DECIMAL(18, 4)               DEFAULT 0,
    UnemploymentInsurance DECIMAL(18, 4)               DEFAULT 0,
    UnionDues             DECIMAL(18, 4)               DEFAULT 0,
    OtherDeductions       DECIMAL(18, 4)               DEFAULT 0,
    TotalDeductions       DECIMAL(18, 4)   NOT NULL,
    NetPay                DECIMAL(18, 4)   NOT NULL,
    PaymentDate           DATE NULL,
    PaymentMethod         INT                          DEFAULT 0, -- 0=BankTransfer, 1=Cash, 2=Check
    Status                INT                          DEFAULT 0,
    FOREIGN KEY (PayrollRunId) REFERENCES PayrollRuns (Id),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id)
);

PRINT
'✅ Module 7: Payroll tables created.';
GO

-- ============================================================
-- MODULE 8: FINANCIAL MANAGEMENT (MALİ İŞLER)
-- ============================================================
PRINT '📦 Module 8: Creating Financial Management tables...';
GO

-- 8.1 Bank Accounts
CREATE TABLE BankAccounts
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccountNumber NVARCHAR(50) NOT NULL UNIQUE,
    BankName      NVARCHAR(100) NOT NULL,
    BankBranch    NVARCHAR(100) NULL,
    IBAN          NVARCHAR(50) NOT NULL UNIQUE,
    Currency      NVARCHAR(10) DEFAULT 'TRY',
    Balance       DECIMAL(18, 4)               DEFAULT 0,
    AccountType   NVARCHAR(50) NOT NULL, -- Checking, Savings, etc.
    IsActive      BIT                          DEFAULT 1,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE()
);

-- 8.2 General Ledger Accounts
CREATE TABLE GeneralLedgerAccounts
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccountCode     NVARCHAR(50) NOT NULL UNIQUE,
    AccountName     NVARCHAR(200) NOT NULL,
    AccountType     INT NOT NULL, -- 0=Asset, 1=Liability, 2=Equity, 3=Revenue, 4=Expense
    ParentAccountId UNIQUEIDENTIFIER NULL,
    IsActive        BIT                          DEFAULT 1,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentAccountId) REFERENCES GeneralLedgerAccounts (Id)
);

-- 8.3 Transactions
CREATE TABLE Transactions
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TransactionNumber NVARCHAR(50) NOT NULL UNIQUE,
    TransactionDate   DATETIME2        NOT NULL,
    TransactionType   INT              NOT NULL,              -- 0=Income, 1=Expense, 2=Transfer
    Amount            DECIMAL(18, 4)   NOT NULL,
    Currency          NVARCHAR(10) DEFAULT 'TRY',
    Description       NVARCHAR(MAX) NOT NULL,
    BankAccountId     UNIQUEIDENTIFIER NULL,
    GLAccountId       UNIQUEIDENTIFIER NULL,
    ReferenceNumber   NVARCHAR(100) NULL,
    Status            INT                          DEFAULT 0, -- 0=Pending, 1=Completed, 2=Cancelled
    CreatedBy         UNIQUEIDENTIFIER NOT NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (BankAccountId) REFERENCES BankAccounts (Id),
    FOREIGN KEY (GLAccountId) REFERENCES GeneralLedgerAccounts (Id),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id)
);

-- 8.4 Budget Allocations
CREATE TABLE BudgetAllocations
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FiscalYear      INT              NOT NULL,
    DepartmentId    UNIQUEIDENTIFIER NOT NULL,
    Category        NVARCHAR(100) NOT NULL,
    AllocatedAmount DECIMAL(18, 4)   NOT NULL,
    SpentAmount     DECIMAL(18, 4)               DEFAULT 0,
    RemainingAmount AS (AllocatedAmount - SpentAmount) PERSISTED,
    Status          INT                          DEFAULT 0,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id)
);

-- 8.5 Expenses
CREATE TABLE Expenses
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ExpenseNumber NVARCHAR(50) NOT NULL UNIQUE,
    ExpenseDate   DATE             NOT NULL,
    DepartmentId  UNIQUEIDENTIFIER NOT NULL,
    Category      NVARCHAR(100) NOT NULL,
    Amount        DECIMAL(18, 4)   NOT NULL,
    Description   NVARCHAR(MAX) NOT NULL,
    RequestedBy   UNIQUEIDENTIFIER NOT NULL,
    ApprovedBy    UNIQUEIDENTIFIER NULL,
    ApprovalDate  DATETIME2 NULL,
    Status        INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected, 3=Paid
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (RequestedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 8.6 Revolving Fund Income
CREATE TABLE RevolvingFundIncome
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IncomeDate  DATE           NOT NULL,
    ProjectId   UNIQUEIDENTIFIER NULL,
    Amount      DECIMAL(18, 4) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    SourceType  NVARCHAR(100) NOT NULL,
    Status      INT                          DEFAULT 0,
    CreatedAt   DATETIME2                    DEFAULT GETUTCDATE()
);

-- 8.7 Revolving Fund Distribution
CREATE TABLE RevolvingFundDistribution
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DistributionMonth INT              NOT NULL,
    DistributionYear  INT              NOT NULL,
    EmployeeId        UNIQUEIDENTIFIER NOT NULL,
    ProjectShare      DECIMAL(18, 4)   NOT NULL,
    DistributionRate  DECIMAL(5, 2)    NOT NULL,
    TotalAmount       DECIMAL(18, 4)   NOT NULL,
    Status            INT                          DEFAULT 0, -- 0=Calculated, 1=Approved, 2=Paid
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id)
);

-- 8.8 Student Fees
CREATE TABLE StudentFees
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId  UNIQUEIDENTIFIER NOT NULL,
    SemesterId UNIQUEIDENTIFIER NOT NULL,
    FeeType    INT              NOT NULL,              -- 0=Tuition, 1=PerCourse, 2=Summer, 3=Lab, 4=Document
    Amount     DECIMAL(18, 4)   NOT NULL,
    DueDate    DATE             NOT NULL,
    PaidAmount DECIMAL(18, 4)               DEFAULT 0,
    RemainingAmount AS (Amount - PaidAmount) PERSISTED,
    Status     INT                          DEFAULT 0, -- 0=Pending, 1=PartiallyPaid, 2=FullyPaid, 3=Overdue
    CreatedAt  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (SemesterId) REFERENCES Semesters (Id)
);

PRINT
'✅ Module 8: Financial Management tables created.';
GO

-- ============================================================
-- MODULE 9: WALLET SYSTEM (KART VE CÜZDAN SİSTEMİ)
-- ============================================================
PRINT '📦 Module 9: Creating Wallet System tables...';
GO

-- 9.1 Access Cards (Digital ID Card)
CREATE TABLE AccessCards
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId          UNIQUEIDENTIFIER NOT NULL,
    CardNumber        NVARCHAR(50) NOT NULL UNIQUE,
    QRCode            NVARCHAR(500) NOT NULL UNIQUE,
    Barcode           NVARCHAR(100) NOT NULL UNIQUE,
    IssueDate         DATE             NOT NULL,
    ExpiryDate        DATE             NOT NULL,
    CardStatus        INT                          DEFAULT 0, -- 0=Active, 1=Blocked, 2=Lost, 3=Expired
    BlockReason       NVARCHAR(500) NULL,
    BlockedAt         DATETIME2 NULL,
    BlockedBy         UNIQUEIDENTIFIER NULL,
    ReplacementCardId UNIQUEIDENTIFIER NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (BlockedBy) REFERENCES Persons (Id)
);

-- 9.2 Wallets
CREATE TABLE Wallets
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId            UNIQUEIDENTIFIER NOT NULL UNIQUE,
    Balance             DECIMAL(18, 4)               DEFAULT 0,
    Currency            NVARCHAR(10) DEFAULT 'TRY',
    LastTransactionDate DATETIME2 NULL,
    IsActive            BIT                          DEFAULT 1,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 9.3 Wallet Transaction History
CREATE TABLE WalletTransactionHistory
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WalletId        UNIQUEIDENTIFIER NOT NULL,
    TransactionType INT              NOT NULL, -- 0=Load, 1=Purchase, 2=Refund, 3=Transfer
    Amount          DECIMAL(18, 4)   NOT NULL,
    BalanceBefore   DECIMAL(18, 4)   NOT NULL,
    BalanceAfter    DECIMAL(18, 4)   NOT NULL,
    Description     NVARCHAR(MAX) NULL,
    MerchantName    NVARCHAR(200) NULL,
    MerchantType    NVARCHAR(100) NULL,
    PaymentMethod   INT NULL,                  -- 0=CreditCard, 1=Cash, 2=BankTransfer
    ReferenceNumber NVARCHAR(100) NULL,
    TransactionDate DATETIME2                    DEFAULT GETUTCDATE(),
    Status          INT                          DEFAULT 0,
    FOREIGN KEY (WalletId) REFERENCES Wallets (Id)
);

PRINT
'✅ Module 9: Wallet System tables created.';
GO

-- ============================================================
-- MODULE 10: SECURITY & ACCESS CONTROL (GÜVENLİK SİSTEMİ)
-- ============================================================
PRINT '📦 Module 10: Creating Security & Access Control tables...';
GO

-- 10.1 Access Points
CREATE TABLE AccessPoints
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccessPointCode  NVARCHAR(50) NOT NULL UNIQUE,
    AccessPointName  NVARCHAR(200) NOT NULL,
    Location         NVARCHAR(200) NOT NULL,
    DeviceType       INT NOT NULL, -- 0=Turnstile, 1=Door, 2=Gate, 3=Reader
    IPAddress        NVARCHAR(50) NULL,
    IsEntry          BIT                          DEFAULT 1,
    IsExit           BIT                          DEFAULT 1,
    RequiresApproval BIT                          DEFAULT 0,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE()
);

-- 10.2 Access Groups
CREATE TABLE AccessGroups
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupName   NVARCHAR(200) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive    BIT                          DEFAULT 1,
    CreatedAt   DATETIME2                    DEFAULT GETUTCDATE()
);

-- 10.3 Access Group Permissions
CREATE TABLE AccessGroupPermissions
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccessGroupId UNIQUEIDENTIFIER NOT NULL,
    AccessPointId UNIQUEIDENTIFIER NOT NULL,
    AllowedDays   NVARCHAR(50) NOT NULL, -- Comma-separated: 1,2,3,4,5
    StartTime     TIME             NOT NULL,
    EndTime       TIME             NOT NULL,
    IsActive      BIT                          DEFAULT 1,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (AccessGroupId) REFERENCES AccessGroups (Id),
    FOREIGN KEY (AccessPointId) REFERENCES AccessPoints (Id)
);

-- 10.4 Access Schedule Exceptions
CREATE TABLE AccessScheduleExceptions
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId      UNIQUEIDENTIFIER NOT NULL,
    AccessPointId UNIQUEIDENTIFIER NOT NULL,
    ExceptionDate DATE             NOT NULL,
    StartTime     TIME             NOT NULL,
    EndTime       TIME             NOT NULL,
    Reason        NVARCHAR(MAX) NULL,
    ApprovedBy    UNIQUEIDENTIFIER NULL,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (AccessPointId) REFERENCES AccessPoints (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 10.5 Access Logs
CREATE TABLE AccessLogs
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId      UNIQUEIDENTIFIER NOT NULL,
    AccessPointId UNIQUEIDENTIFIER NOT NULL,
    AccessCardId  UNIQUEIDENTIFIER NULL,
    AccessType    INT              NOT NULL, -- 0=Entry, 1=Exit
    AccessMethod  INT              NOT NULL, -- 0=QRCode, 1=Card, 2=Biometric, 3=Manual
    AccessTime    DATETIME2                    DEFAULT GETUTCDATE(),
    IsAuthorized  BIT                          DEFAULT 1,
    DenialReason  NVARCHAR(500) NULL,
    Temperature   DECIMAL(5, 2) NULL,
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (AccessPointId) REFERENCES AccessPoints (Id),
    FOREIGN KEY (AccessCardId) REFERENCES AccessCards (Id)
);

-- 10.6 Cameras
CREATE TABLE Cameras
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CameraCode          NVARCHAR(50) NOT NULL UNIQUE,
    CameraName          NVARCHAR(200) NOT NULL,
    Location            NVARCHAR(200) NOT NULL,
    IPAddress           NVARCHAR(50) NULL,
    CameraType          NVARCHAR(100) NOT NULL, -- Indoor, Outdoor, PTZ, Dome
    StreamURL           NVARCHAR(500) NULL,
    IsActive            BIT                          DEFAULT 1,
    LastMaintenanceDate DATE NULL,
    NextMaintenanceDate DATE NULL,
    Status              INT                          DEFAULT 0,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE()
);

-- 10.7 Security Incidents
CREATE TABLE SecurityIncidents
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IncidentNumber NVARCHAR(50) NOT NULL UNIQUE,
    IncidentDate   DATETIME2        NOT NULL,
    IncidentType   NVARCHAR(100) NOT NULL,
    Location       NVARCHAR(200) NOT NULL,
    Description    NVARCHAR(MAX) NOT NULL,
    Severity       INT              NOT NULL,              -- 0=Low, 1=Medium, 2=High, 3=Critical
    ReportedBy     UNIQUEIDENTIFIER NOT NULL,
    AssignedTo     UNIQUEIDENTIFIER NULL,
    Status         INT                          DEFAULT 0, -- 0=New, 1=UnderInvestigation, 2=Resolved, 3=Closed
    Resolution     NVARCHAR(MAX) NULL,
    ResolvedDate   DATETIME2 NULL,
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ReportedBy) REFERENCES Persons (Id),
    FOREIGN KEY (AssignedTo) REFERENCES Persons (Id)
);

-- 10.8 Visitor Log
CREATE TABLE VisitorLog
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VisitorName          NVARCHAR(200) NOT NULL,
    VisitorIdNumber      NVARCHAR(50) NOT NULL,
    VisitorPhone         NVARCHAR(20) NULL,
    VisitorCompany       NVARCHAR(200) NULL,
    VisitPurpose         NVARCHAR(MAX) NOT NULL,
    VisitingPersonId     UNIQUEIDENTIFIER NULL,
    VisitingDepartmentId UNIQUEIDENTIFIER NULL,
    CheckInTime          DATETIME2 NOT NULL,
    CheckOutTime         DATETIME2 NULL,
    TemporaryCardNumber  NVARCHAR(50) NULL,
    EscortRequired       BIT                          DEFAULT 0,
    Status               INT                          DEFAULT 0, -- 0=InBuilding, 1=CheckedOut, 2=Expired
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (VisitingPersonId) REFERENCES Persons (Id),
    FOREIGN KEY (VisitingDepartmentId) REFERENCES Departments (Id)
);

-- 10.9 Emergency Alerts
CREATE TABLE EmergencyAlerts
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AlertType       INT       NOT NULL, -- 0=Fire, 1=Earthquake, 2=Security, 3=Medical, 4=Other
    AlertDate       DATETIME2 NOT NULL,
    Location        NVARCHAR(200) NOT NULL,
    Description     NVARCHAR(MAX) NOT NULL,
    Severity        INT       NOT NULL, -- 0=Low, 1=Medium, 2=High, 3=Critical
    IsActive        BIT                          DEFAULT 1,
    TriggeredBy     UNIQUEIDENTIFIER NULL,
    ResolvedDate    DATETIME2 NULL,
    ResolutionNotes NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (TriggeredBy) REFERENCES Persons (Id)
);

-- 10.10 Evacuation Records
CREATE TABLE EvacuationRecords
(
    Id                     UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmergencyAlertId       UNIQUEIDENTIFIER NOT NULL,
    BuildingId             UNIQUEIDENTIFIER NULL,
    TotalPersonsInBuilding INT              NOT NULL,
    PersonsEvacuated       INT                          DEFAULT 0,
    PersonsMissing         INT                          DEFAULT 0,
    EvacuationStartTime    DATETIME2        NOT NULL,
    EvacuationEndTime      DATETIME2 NULL,
    AssemblyPoint          NVARCHAR(200) NOT NULL,
    Status                 INT                          DEFAULT 0, -- 0=InProgress, 1=Completed, 2=Aborted
    Notes                  NVARCHAR(MAX) NULL,
    CreatedAt              DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmergencyAlertId) REFERENCES EmergencyAlerts (Id)
);

PRINT
'✅ Module 10: Security & Access Control tables created.';
GO

-- ============================================================
-- MODULE 11: PARKING MANAGEMENT (PARK YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 11: Creating Parking Management tables...';
GO

-- 11.1 Parking Lots
CREATE TABLE ParkingLots
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingLotCode  NVARCHAR(20) NOT NULL UNIQUE,
    ParkingLotName  NVARCHAR(200) NOT NULL,
    Location        NVARCHAR(200) NOT NULL,
    TotalSpaces     INT NOT NULL,
    AvailableSpaces INT NOT NULL,
    LotType         INT NOT NULL,                           -- 0=Student, 1=Staff, 2=Visitor, 3=Mixed, 4=Disabled
    HasANPR         BIT                          DEFAULT 0, -- Automatic Number Plate Recognition
    IsActive        BIT                          DEFAULT 1,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE()
);

-- 11.2 Parking Spaces
CREATE TABLE ParkingSpaces
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ParkingLotId       UNIQUEIDENTIFIER NOT NULL,
    SpaceNumber        NVARCHAR(20) NOT NULL,
    SpaceType          INT              NOT NULL, -- 0=Regular, 1=Reserved, 2=Disabled, 3=Electric, 4=Motorcycle
    IsOccupied         BIT                          DEFAULT 0,
    AssignedToPersonId UNIQUEIDENTIFIER NULL,
    MonthlyRate        DECIMAL(18, 4) NULL,
    HourlyRate         DECIMAL(18, 4) NULL,
    FOREIGN KEY (ParkingLotId) REFERENCES ParkingLots (Id),
    FOREIGN KEY (AssignedToPersonId) REFERENCES Persons (Id)
);

-- 11.3 Vehicle Registration
CREATE TABLE VehicleRegistration
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId         UNIQUEIDENTIFIER NOT NULL,
    LicensePlate     NVARCHAR(20) NOT NULL UNIQUE,
    VehicleType      INT              NOT NULL, -- 0=Car, 1=Motorcycle, 2=Bicycle
    Brand            NVARCHAR(100) NULL,
    Model            NVARCHAR(100) NULL,
    Color            NVARCHAR(50) NULL,
    RegistrationDate DATE             NOT NULL,
    ExpiryDate       DATE NULL,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 11.4 Parking Cards
CREATE TABLE ParkingCards
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VehicleRegistrationId UNIQUEIDENTIFIER NOT NULL,
    CardNumber            NVARCHAR(50) NOT NULL UNIQUE,
    CardType              INT              NOT NULL,              -- 0=Monthly, 1=Semester, 2=Annual, 3=Daily
    IssueDate             DATE             NOT NULL,
    ExpiryDate            DATE             NOT NULL,
    Status                INT                          DEFAULT 0, -- 0=Active, 1=Expired, 2=Cancelled, 3=Lost
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (VehicleRegistrationId) REFERENCES VehicleRegistration (Id)
);

-- 11.5 Parking Entry/Exit Log
CREATE TABLE ParkingEntryExitLog
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VehicleRegistrationId UNIQUEIDENTIFIER NOT NULL,
    ParkingLotId          UNIQUEIDENTIFIER NOT NULL,
    ParkingSpaceId        UNIQUEIDENTIFIER NULL,
    EntryTime             DATETIME2        NOT NULL,
    ExitTime              DATETIME2 NULL,
    Duration              INT NULL,                               -- in minutes
    ParkingFee            DECIMAL(18, 4)               DEFAULT 0,
    PaymentStatus         INT                          DEFAULT 0, -- 0=Unpaid, 1=Paid
    PaymentMethod         INT NULL,
    DetectionMethod       INT              NOT NULL,              -- 0=ANPR, 1=Manual, 2=Card
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (VehicleRegistrationId) REFERENCES VehicleRegistration (Id),
    FOREIGN KEY (ParkingLotId) REFERENCES ParkingLots (Id),
    FOREIGN KEY (ParkingSpaceId) REFERENCES ParkingSpaces (Id)
);

PRINT
'✅ Module 11: Parking Management tables created.';
GO

-- ============================================================
-- CONTINUING WITH REMAINING MODULES...
-- Due to character limitations, I'll continue in the next part
-- ============================================================

-- ============================================================
-- MODULE 12: PROCUREMENT & SUPPLIERS (SATIN ALMA)
-- ============================================================
PRINT '📦 Module 12: Creating Procurement & Suppliers tables...';
GO

-- 12.1 Suppliers
CREATE TABLE Suppliers
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierCode    NVARCHAR(50) NOT NULL UNIQUE,
    CompanyName     NVARCHAR(200) NOT NULL,
    TaxNumber       NVARCHAR(20) NOT NULL UNIQUE,
    ContactPerson   NVARCHAR(100) NULL,
    PhoneNumber     NVARCHAR(20) NULL,
    Email           NVARCHAR(100) NULL,
    Address         NVARCHAR(MAX) NULL,
    City            NVARCHAR(100) NULL,
    Country         NVARCHAR(100) DEFAULT 'Turkey',
    SupplierType    NVARCHAR(100) NULL,
    PaymentTerms    NVARCHAR(200) NULL,
    Rating          DECIMAL(3, 2)                DEFAULT 0.00,
    IsApproved      BIT                          DEFAULT 0,
    IsBlacklisted   BIT                          DEFAULT 0,
    BlacklistReason NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE()
);

-- 12.2 Purchase Requests
CREATE TABLE PurchaseRequests
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequestNumber      NVARCHAR(50) NOT NULL UNIQUE,
    RequestDate        DATE             NOT NULL,
    RequestedBy        UNIQUEIDENTIFIER NOT NULL,
    DepartmentId       UNIQUEIDENTIFIER NOT NULL,
    Priority           INT                          DEFAULT 1, -- 0=Low, 1=Normal, 2=High, 3=Urgent
    TotalEstimatedCost DECIMAL(18, 4)   NOT NULL,
    Justification      NVARCHAR(MAX) NULL,
    Status             INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Rejected, 3=Ordered
    ApprovedBy         UNIQUEIDENTIFIER NULL,
    ApprovalDate       DATETIME2 NULL,
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (RequestedBy) REFERENCES Persons (Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 12.3 Purchase Orders
CREATE TABLE PurchaseOrders
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PONumber          NVARCHAR(50) NOT NULL UNIQUE,
    PurchaseRequestId UNIQUEIDENTIFIER NULL,
    SupplierId        UNIQUEIDENTIFIER NOT NULL,
    OrderDate         DATE             NOT NULL,
    DeliveryDate      DATE NULL,
    TotalAmount       DECIMAL(18, 4)   NOT NULL,
    Currency          NVARCHAR(10) DEFAULT 'TRY',
    PaymentTerms      NVARCHAR(200) NULL,
    Status            INT                          DEFAULT 0, -- 0=Draft, 1=Sent, 2=Acknowledged, 3=PartiallyReceived, 4=Received, 5=Cancelled
    CreatedBy         UNIQUEIDENTIFIER NOT NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PurchaseRequestId) REFERENCES PurchaseRequests (Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers (Id),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id)
);

-- 12.4 Tenders (İhaleler)
CREATE TABLE Tenders
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenderNumber          NVARCHAR(50) NOT NULL UNIQUE,
    TenderTitle           NVARCHAR(300) NOT NULL,
    TenderType            INT            NOT NULL,                -- 0=OpenTender, 1=RestrictedTender, 2=Negotiation, 3=DirectProcurement
    Description           NVARCHAR(MAX) NOT NULL,
    EstimatedAmount       DECIMAL(18, 4) NOT NULL,
    AnnouncementDate      DATE           NOT NULL,
    BidSubmissionDeadline DATETIME2      NOT NULL,
    TenderOpeningDate     DATETIME2      NOT NULL,
    Status                INT                          DEFAULT 0, -- 0=Announced, 1=BiddingOpen, 2=BiddingClosed, 3=UnderEvaluation, 4=Awarded, 5=Cancelled
    IsDeleted             BIT                          DEFAULT 0,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE()
);

-- 12.5 Tender Bids
CREATE TABLE TenderBids
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenderId       UNIQUEIDENTIFIER NOT NULL,
    SupplierId     UNIQUEIDENTIFIER NOT NULL,
    BidAmount      DECIMAL(18, 4)   NOT NULL,
    BidDate        DATETIME2        NOT NULL,
    TechnicalScore DECIMAL(5, 2) NULL,
    FinancialScore DECIMAL(5, 2) NULL,
    TotalScore     DECIMAL(5, 2) NULL,
    IsWinner       BIT                          DEFAULT 0,
    Status         INT                          DEFAULT 0, -- 0=Submitted, 1=UnderReview, 2=Accepted, 3=Rejected
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenderId) REFERENCES Tenders (Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers (Id)
);

-- 12.6 Tender Documents
CREATE TABLE TenderDocuments
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenderId     UNIQUEIDENTIFIER NOT NULL,
    DocumentName NVARCHAR(300) NOT NULL,
    DocumentType NVARCHAR(100) NOT NULL, -- TechnicalSpec, BiddingGuidelines, Contract, etc.
    FilePath     NVARCHAR(500) NULL,
    FileSize     BIGINT NULL,
    UploadDate   DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenderId) REFERENCES Tenders (Id)
);

-- 12.7 Tender Evaluation Committee
CREATE TABLE TenderEvaluationCommittee
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenderId          UNIQUEIDENTIFIER NOT NULL,
    CommitteeMemberId UNIQUEIDENTIFIER NOT NULL,
    Role              NVARCHAR(100) NOT NULL, -- Chairman, Member, Observer
    AppointmentDate   DATE             NOT NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenderId) REFERENCES Tenders (Id),
    FOREIGN KEY (CommitteeMemberId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 12: Procurement & Suppliers tables created.';
GO

-- ============================================================
-- MODULE 13: INVENTORY MANAGEMENT (STOK YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 13: Creating Inventory Management tables...';
GO

-- 13.1 Warehouses (Kampüs Bazlı Depolar)
CREATE TABLE Warehouses
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WarehouseCode NVARCHAR(20) NOT NULL UNIQUE,
    WarehouseName NVARCHAR(200) NOT NULL,
    Location      NVARCHAR(200) NOT NULL,
    WarehouseType NVARCHAR(100) NOT NULL, -- Main, Sub, Department
    ManagerId     UNIQUEIDENTIFIER NULL,
    Capacity      DECIMAL(10, 2) NULL,    -- in cubic meters
    IsActive      BIT                          DEFAULT 1,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ManagerId) REFERENCES Persons (Id)
);

-- 13.2 Item Categories
CREATE TABLE ItemCategories
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryCode     NVARCHAR(20) NOT NULL UNIQUE,
    CategoryName     NVARCHAR(200) NOT NULL,
    ParentCategoryId UNIQUEIDENTIFIER NULL,
    Description      NVARCHAR(MAX) NULL,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentCategoryId) REFERENCES ItemCategories (Id)
);

-- 13.3 Items (Malzemeler)
CREATE TABLE Items
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ItemCode          NVARCHAR(50) NOT NULL UNIQUE,
    ItemName          NVARCHAR(200) NOT NULL,
    CategoryId        UNIQUEIDENTIFIER NOT NULL,
    Description       NVARCHAR(MAX) NULL,
    UnitOfMeasure     NVARCHAR(50) NOT NULL,
    MinimumStockLevel INT                          DEFAULT 0,
    ReorderPoint      INT                          DEFAULT 0,
    UnitPrice         DECIMAL(18, 4) NULL,
    IsActive          BIT                          DEFAULT 1,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CategoryId) REFERENCES ItemCategories (Id)
);

-- 13.4 Stock (Warehouse-specific inventory)
CREATE TABLE Stock
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ItemId           UNIQUEIDENTIFIER NOT NULL,
    WarehouseId      UNIQUEIDENTIFIER NOT NULL,
    Quantity         INT              NOT NULL    DEFAULT 0,
    ReservedQuantity INT                          DEFAULT 0,
    AvailableQuantity AS (Quantity - ReservedQuantity) PERSISTED,
    LastUpdated      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ItemId) REFERENCES Items (Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id)
);

-- 13.5 Stock Movements
CREATE TABLE StockMovements
(
    Id                     UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ItemId                 UNIQUEIDENTIFIER NOT NULL,
    WarehouseId            UNIQUEIDENTIFIER NOT NULL,
    MovementType           INT              NOT NULL, -- 0=In, 1=Out, 2=Transfer, 3=Adjustment
    Quantity               INT              NOT NULL,
    MovementDate           DATETIME2                    DEFAULT GETUTCDATE(),
    ReferenceNumber        NVARCHAR(100) NULL,
    SourceWarehouseId      UNIQUEIDENTIFIER NULL,
    DestinationWarehouseId UNIQUEIDENTIFIER NULL,
    Reason                 NVARCHAR(MAX) NULL,
    PerformedBy            UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (ItemId) REFERENCES Items (Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id),
    FOREIGN KEY (SourceWarehouseId) REFERENCES Warehouses (Id),
    FOREIGN KEY (DestinationWarehouseId) REFERENCES Warehouses (Id),
    FOREIGN KEY (PerformedBy) REFERENCES Persons (Id)
);

-- 13.6 Stock Count (Inventory Checks)
CREATE TABLE StockCounts
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CountNumber NVARCHAR(50) NOT NULL UNIQUE,
    WarehouseId UNIQUEIDENTIFIER NOT NULL,
    CountDate   DATE             NOT NULL,
    CountType   INT              NOT NULL,              -- 0=Full, 1=Partial, 2=Cycle
    Status      INT                          DEFAULT 0, -- 0=InProgress, 1=Completed, 2=Cancelled
    CountedBy   UNIQUEIDENTIFIER NOT NULL,
    ApprovedBy  UNIQUEIDENTIFIER NULL,
    Notes       NVARCHAR(MAX) NULL,
    CreatedAt   DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses (Id),
    FOREIGN KEY (CountedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 13.7 Assets (Demirbaşlar)
CREATE TABLE Assets
(
    Id                     UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AssetNumber            NVARCHAR(50) NOT NULL UNIQUE,
    AssetName              NVARCHAR(200) NOT NULL,
    CategoryId             UNIQUEIDENTIFIER NOT NULL,
    PurchaseDate           DATE             NOT NULL,
    PurchaseCost           DECIMAL(18, 4)   NOT NULL,
    CurrentValue           DECIMAL(18, 4) NULL,
    Location               NVARCHAR(200) NULL,
    AssignedToPersonId     UNIQUEIDENTIFIER NULL,
    AssignedToDepartmentId UNIQUEIDENTIFIER NULL,
    SerialNumber           NVARCHAR(100) NULL,
    Brand                  NVARCHAR(100) NULL,
    Model                  NVARCHAR(100) NULL,
    WarrantyExpiry         DATE NULL,
    DepreciationRate       DECIMAL(5, 2)                DEFAULT 0,
    Status                 INT                          DEFAULT 0, -- 0=Active, 1=InRepair, 2=Retired, 3=Lost
    CreatedAt              DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CategoryId) REFERENCES ItemCategories (Id),
    FOREIGN KEY (AssignedToPersonId) REFERENCES Persons (Id),
    FOREIGN KEY (AssignedToDepartmentId) REFERENCES Departments (Id)
);

PRINT
'✅ Module 13: Inventory Management tables created.';
GO

-- ============================================================
-- MODULE 14: LIBRARY MANAGEMENT (KÜTÜPHANE YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 14: Creating Library Management tables...';
GO

-- 14.1 Library Categories
CREATE TABLE LibraryCategories
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryCode     NVARCHAR(20) NOT NULL UNIQUE,
    CategoryName     NVARCHAR(200) NOT NULL,
    ParentCategoryId UNIQUEIDENTIFIER NULL,
    Description      NVARCHAR(MAX) NULL,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentCategoryId) REFERENCES LibraryCategories (Id)
);

-- 14.2 Books
CREATE TABLE Books
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ISBN            NVARCHAR(20) NOT NULL UNIQUE,
    Title           NVARCHAR(300) NOT NULL,
    Author          NVARCHAR(200) NOT NULL,
    Publisher       NVARCHAR(200) NULL,
    PublicationYear INT NULL,
    Edition         NVARCHAR(50) NULL,
    CategoryId      UNIQUEIDENTIFIER NOT NULL,
    LanguageCode    NVARCHAR(10) DEFAULT 'TR',
    PageCount       INT NULL,
    TotalCopies     INT              NOT NULL    DEFAULT 1,
    AvailableCopies INT              NOT NULL    DEFAULT 1,
    Location        NVARCHAR(200) NULL,
    Description     NVARCHAR(MAX) NULL,
    CoverImagePath  NVARCHAR(500) NULL,
    Status          INT                          DEFAULT 0, -- 0=Available, 1=OutOfStock, 2=Discontinued
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CategoryId) REFERENCES LibraryCategories (Id)
);

-- 14.3 Book Reservations
CREATE TABLE BookReservations
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookId           UNIQUEIDENTIFIER NOT NULL,
    PersonId         UNIQUEIDENTIFIER NOT NULL,
    ReservationDate  DATETIME2                    DEFAULT GETUTCDATE(),
    ExpiryDate       DATETIME2        NOT NULL,
    Status           INT                          DEFAULT 0, -- 0=Active, 1=Fulfilled, 2=Expired, 3=Cancelled
    NotificationSent BIT                          DEFAULT 0,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookId) REFERENCES Books (Id),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 14.4 Loans (Ödünç Verme)
CREATE TABLE Loans
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BookId       UNIQUEIDENTIFIER NOT NULL,
    PersonId     UNIQUEIDENTIFIER NOT NULL,
    LoanDate     DATE             NOT NULL,
    DueDate      DATE             NOT NULL,
    ReturnDate   DATE NULL,
    RenewalCount INT                          DEFAULT 0,
    Status       INT                          DEFAULT 0, -- 0=Active, 1=Returned, 2=Overdue, 3=Lost
    IssuedBy     UNIQUEIDENTIFIER NOT NULL,
    ReturnedTo   UNIQUEIDENTIFIER NULL,
    Notes        NVARCHAR(MAX) NULL,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookId) REFERENCES Books (Id),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (IssuedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ReturnedTo) REFERENCES Persons (Id)
);

-- 14.5 Library Fines
CREATE TABLE LibraryFines
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId   UNIQUEIDENTIFIER NOT NULL,
    LoanId     UNIQUEIDENTIFIER NULL,
    FineType   INT              NOT NULL,              -- 0=LateFee, 1=DamageFee, 2=LostBook
    Amount     DECIMAL(18, 4)   NOT NULL,
    FineDate   DATE             NOT NULL,
    DueDate    DATE             NOT NULL,
    PaidDate   DATE NULL,
    PaidAmount DECIMAL(18, 4)               DEFAULT 0,
    Status     INT                          DEFAULT 0, -- 0=Unpaid, 1=PartiallyPaid, 2=FullyPaid, 3=Waived
    CreatedAt  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (LoanId) REFERENCES Loans (Id)
);

-- 14.6 E-Books
CREATE TABLE EBooks
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title           NVARCHAR(300) NOT NULL,
    Author          NVARCHAR(200) NOT NULL,
    ISBN            NVARCHAR(20) NULL,
    Publisher       NVARCHAR(200) NULL,
    PublicationYear INT NULL,
    CategoryId      UNIQUEIDENTIFIER NOT NULL,
    LanguageCode    NVARCHAR(10) DEFAULT 'TR',
    FilePath        NVARCHAR(500) NULL,
    FileURL         NVARCHAR(500) NULL,
    FileFormat      NVARCHAR(20) NULL,                      -- PDF, EPUB, MOBI
    FileSize        BIGINT NULL,
    DownloadCount   INT                          DEFAULT 0,
    AccessLevel     INT                          DEFAULT 0, -- 0=Public, 1=Restricted, 2=Subscription
    Status          INT                          DEFAULT 0,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CategoryId) REFERENCES LibraryCategories (Id)
);

-- 14.7 Journals
CREATE TABLE Journals
(
    Id                    UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    JournalTitle          NVARCHAR(300) NOT NULL,
    ISSN                  NVARCHAR(20) NULL,
    Publisher             NVARCHAR(200) NOT NULL,
    Subject               NVARCHAR(200) NOT NULL,
    SubscriptionStartDate DATE NOT NULL,
    SubscriptionEndDate   DATE NOT NULL,
    AccessType            INT  NOT NULL, -- 0=Print, 1=Online, 2=Both
    AccessURL             NVARCHAR(500) NULL,
    AnnualCost            DECIMAL(18, 4) NULL,
    Status                INT                          DEFAULT 0,
    CreatedAt             DATETIME2                    DEFAULT GETUTCDATE()
);

-- 14.8 Theses
CREATE TABLE Theses
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ThesisTitle   NVARCHAR(500) NOT NULL,
    Author        NVARCHAR(200) NOT NULL,
    DepartmentId  UNIQUEIDENTIFIER NOT NULL,
    ThesisType    INT              NOT NULL,              -- 0=Bachelor, 1=Masters, 2=PhD
    Year          INT              NOT NULL,
    AdvisorId     UNIQUEIDENTIFIER NULL,
    AbstractText  NVARCHAR(MAX) NULL,
    Keywords      NVARCHAR(MAX) NULL,
    FilePath      NVARCHAR(500) NULL,
    AccessLevel   INT                          DEFAULT 0, -- 0=Public, 1=Restricted, 2=Confidential
    DownloadCount INT                          DEFAULT 0,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (AdvisorId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 14: Library Management tables created.';
GO

-- ============================================================
-- MODULE 15: CAFETERIA & CATERING (YEMEKHANE/KAFETERİA)
-- ============================================================
PRINT '📦 Module 15: Creating Cafeteria & Catering tables...';
GO

-- 15.1 Cafeterias
CREATE TABLE Cafeterias
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaCode NVARCHAR(20) NOT NULL UNIQUE,
    CafeteriaName NVARCHAR(200) NOT NULL,
    Location      NVARCHAR(200) NOT NULL,
    Capacity      INT  NOT NULL,
    ManagerId     UNIQUEIDENTIFIER NULL,
    CafeteriaType INT  NOT NULL, -- 0=MainCafeteria, 1=SmallCafe, 2=FastFood, 3=Restaurant
    OpeningTime   TIME NOT NULL,
    ClosingTime   TIME NOT NULL,
    Status        INT                          DEFAULT 0,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ManagerId) REFERENCES Persons (Id)
);

-- 15.2 Menus
CREATE TABLE Menus
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId UNIQUEIDENTIFIER NOT NULL,
    MenuDate    DATE             NOT NULL,
    MealType    INT              NOT NULL, -- 0=Breakfast, 1=Lunch, 2=Dinner
    IsActive    BIT                          DEFAULT 1,
    CreatedAt   DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CafeteriaId) REFERENCES Cafeterias (Id)
);

-- 15.3 Menu Items
CREATE TABLE MenuItems
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MenuId       UNIQUEIDENTIFIER NOT NULL,
    ItemName     NVARCHAR(200) NOT NULL,
    Description  NVARCHAR(MAX) NULL,
    Calories     INT NULL,
    Price        DECIMAL(18, 4)   NOT NULL,
    IsVegetarian BIT                          DEFAULT 0,
    IsVegan      BIT                          DEFAULT 0,
    IsGlutenFree BIT                          DEFAULT 0,
    Allergens    NVARCHAR(MAX) NULL,
    ImagePath    NVARCHAR(500) NULL,
    FOREIGN KEY (MenuId) REFERENCES Menus (Id)
);

-- 15.4 Cafeteria Products
CREATE TABLE CafeteriaProducts
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId   UNIQUEIDENTIFIER NOT NULL,
    ProductCode   NVARCHAR(50) NOT NULL UNIQUE,
    ProductName   NVARCHAR(200) NOT NULL,
    Category      NVARCHAR(100) NOT NULL, -- Beverage, Snack, Main Course, etc.
    Price         DECIMAL(18, 4)   NOT NULL,
    StockQuantity INT                          DEFAULT 0,
    MinimumStock  INT                          DEFAULT 0,
    IsActive      BIT                          DEFAULT 1,
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CafeteriaId) REFERENCES Cafeterias (Id)
);

-- 15.5 Cafeteria Sales
CREATE TABLE CafeteriaSales
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CafeteriaId          UNIQUEIDENTIFIER NOT NULL,
    ProductId            UNIQUEIDENTIFIER NULL,
    MenuItemId           UNIQUEIDENTIFIER NULL,
    PersonId             UNIQUEIDENTIFIER NOT NULL,
    SaleDate             DATETIME2                    DEFAULT GETUTCDATE(),
    Quantity             INT              NOT NULL,
    UnitPrice            DECIMAL(18, 4)   NOT NULL,
    TotalAmount          DECIMAL(18, 4)   NOT NULL,
    PaymentMethod        INT              NOT NULL, -- 0=Card, 1=Cash, 2=Wallet
    TransactionReference NVARCHAR(100) NULL,
    FOREIGN KEY (CafeteriaId) REFERENCES Cafeterias (Id),
    FOREIGN KEY (ProductId) REFERENCES CafeteriaProducts (Id),
    FOREIGN KEY (MenuItemId) REFERENCES MenuItems (Id),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 15.6 Meal Reservations
CREATE TABLE MealReservations
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId        UNIQUEIDENTIFIER NOT NULL,
    MenuId          UNIQUEIDENTIFIER NOT NULL,
    ReservationDate DATETIME2                    DEFAULT GETUTCDATE(),
    Status          INT                          DEFAULT 0, -- 0=Active, 1=Claimed, 2=Cancelled, 3=NoShow
    IsClaimed       BIT                          DEFAULT 0,
    ClaimTime       DATETIME2 NULL,
    FOREIGN KEY (PersonId) REFERENCES Persons (Id),
    FOREIGN KEY (MenuId) REFERENCES Menus (Id)
);

PRINT
'✅ Module 15: Cafeteria & Catering tables created.';
GO

-- ============================================================
-- MODULE 16: FACILITY MANAGEMENT (TESİS YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 16: Creating Facility Management tables...';
GO

-- 16.1 Buildings
CREATE TABLE Buildings
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BuildingCode      NVARCHAR(20) NOT NULL UNIQUE,
    BuildingName      NVARCHAR(200) NOT NULL,
    Address           NVARCHAR(MAX) NULL,
    TotalFloors       INT NOT NULL,
    TotalRooms        INT NOT NULL,
    TotalArea         DECIMAL(10, 2) NULL, -- square meters
    ConstructionYear  INT NULL,
    HasElevator       BIT                          DEFAULT 0,
    HasDisabledAccess BIT                          DEFAULT 0,
    Status            INT                          DEFAULT 0,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE()
);

-- 16.2 Rooms
CREATE TABLE Rooms
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BuildingId UNIQUEIDENTIFIER NOT NULL,
    RoomCode   NVARCHAR(50) NOT NULL UNIQUE,
    RoomNumber NVARCHAR(50) NOT NULL,
    RoomName   NVARCHAR(200) NULL,
    Floor      INT              NOT NULL,
    RoomType   INT              NOT NULL,              -- 0=Classroom, 1=Lab, 2=Office, 3=MeetingRoom, 4=StudyRoom
    Capacity   INT              NOT NULL,
    Area       DECIMAL(10, 2) NULL,
    Facilities NVARCHAR(MAX) NULL,                     -- Projector, Whiteboard, AC, etc.
    Status     INT                          DEFAULT 0, -- 0=Available, 1=Occupied, 2=UnderMaintenance
    CreatedAt  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (BuildingId) REFERENCES Buildings (Id)
);

-- 16.3 Laboratories
CREATE TABLE Laboratories
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabCode      NVARCHAR(20) NOT NULL UNIQUE,
    LabName      NVARCHAR(200) NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,
    BuildingId   UNIQUEIDENTIFIER NULL,
    RoomId       UNIQUEIDENTIFIER NULL,
    Capacity     INT              NOT NULL,
    LabType      NVARCHAR(100) NOT NULL,                 -- Computer, Chemistry, Physics, Biology
    ManagerId    UNIQUEIDENTIFIER NULL,
    SafetyLevel  INT                          DEFAULT 0, -- 0=Standard, 1=Enhanced, 2=HighSecurity
    Status       INT                          DEFAULT 0,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (BuildingId) REFERENCES Buildings (Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms (Id),
    FOREIGN KEY (ManagerId) REFERENCES Persons (Id)
);

-- 16.4 Lab Equipment
CREATE TABLE LabEquipment
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LaboratoryId        UNIQUEIDENTIFIER NOT NULL,
    EquipmentCode       NVARCHAR(50) NOT NULL UNIQUE,
    EquipmentName       NVARCHAR(200) NOT NULL,
    Model               NVARCHAR(100) NULL,
    SerialNumber        NVARCHAR(100) NULL,
    PurchaseDate        DATE NULL,
    PurchaseCost        DECIMAL(18, 4) NULL,
    WarrantyExpiry      DATE NULL,
    LastCalibrationDate DATE NULL,
    NextCalibrationDate DATE NULL,
    MaintenanceSchedule NVARCHAR(200) NULL,
    Status              INT                          DEFAULT 0, -- 0=Operational, 1=UnderMaintenance, 2=OutOfOrder, 3=Retired
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (LaboratoryId) REFERENCES Laboratories (Id)
);

-- 16.5 Lab Reservations
CREATE TABLE LabReservations
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LaboratoryId         UNIQUEIDENTIFIER NOT NULL,
    ReservedBy           UNIQUEIDENTIFIER NOT NULL,
    ReservationDate      DATE             NOT NULL,
    StartTime            TIME             NOT NULL,
    EndTime              TIME             NOT NULL,
    Purpose              NVARCHAR(MAX) NOT NULL,
    NumberOfParticipants INT                          DEFAULT 1,
    Status               INT                          DEFAULT 0, -- 0=Confirmed, 1=InUse, 2=Completed, 3=Cancelled
    ApprovedBy           UNIQUEIDENTIFIER NULL,
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (LaboratoryId) REFERENCES Laboratories (Id),
    FOREIGN KEY (ReservedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

-- 16.6 Chemical Inventory
CREATE TABLE ChemicalInventory
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LaboratoryId      UNIQUEIDENTIFIER NOT NULL,
    ChemicalName      NVARCHAR(200) NOT NULL,
    CASNumber         NVARCHAR(50) NULL,
    Quantity          DECIMAL(10, 2)   NOT NULL,
    Unit              NVARCHAR(20) NOT NULL,
    HazardClass       NVARCHAR(50) NULL,
    StorageLocation   NVARCHAR(100) NULL,
    StorageConditions NVARCHAR(MAX) NULL,
    ExpiryDate        DATE NULL,
    MSDSFilePath      NVARCHAR(500) NULL,
    Status            INT                          DEFAULT 0,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (LaboratoryId) REFERENCES Laboratories (Id)
);

-- 16.7 Equipment Calibration
CREATE TABLE EquipmentCalibration
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EquipmentId         UNIQUEIDENTIFIER NOT NULL,
    CalibrationDate     DATE             NOT NULL,
    NextCalibrationDate DATE             NOT NULL,
    CalibratedBy        NVARCHAR(200) NOT NULL,
    CalibrationAgency   NVARCHAR(200) NULL,
    Result              NVARCHAR(100) NOT NULL, -- Pass, Fail, Conditional
    CertificateNumber   NVARCHAR(100) NULL,
    CertificatePath     NVARCHAR(500) NULL,
    Cost                DECIMAL(18, 4) NULL,
    Notes               NVARCHAR(MAX) NULL,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EquipmentId) REFERENCES LabEquipment (Id)
);

PRINT
'✅ Module 16: Facility Management tables created.';
GO

-- ============================================================
-- MODULE 17: HEALTH SERVICES (SAĞLIK HİZMETLERİ)
-- ============================================================
PRINT '📦 Module 17: Creating Health Services tables...';
GO

-- 17.1 Health Records
CREATE TABLE HealthRecords
(
    Id                       UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId                 UNIQUEIDENTIFIER NOT NULL UNIQUE,
    BloodType                NVARCHAR(5) NULL,
    RhFactor                 NVARCHAR(10) NULL,
    Allergies                NVARCHAR(MAX) NULL,
    ChronicDiseases          NVARCHAR(MAX) NULL,
    CurrentMedications       NVARCHAR(MAX) NULL,
    EmergencyContactName     NVARCHAR(100) NULL,
    EmergencyContactPhone    NVARCHAR(20) NULL,
    EmergencyContactRelation NVARCHAR(50) NULL,
    CreatedAt                DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt                DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 17.2 Medical Appointments
CREATE TABLE MedicalAppointments
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId        UNIQUEIDENTIFIER NOT NULL,
    AppointmentDate DATETIME2        NOT NULL,
    DoctorName      NVARCHAR(100) NOT NULL,
    Department      NVARCHAR(100) NULL,
    Reason          NVARCHAR(MAX) NULL,
    Status          INT                          DEFAULT 0, -- 0=Scheduled, 1=Completed, 2=Cancelled, 3=NoShow
    Notes           NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 17.3 Prescriptions
CREATE TABLE Prescriptions
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId         UNIQUEIDENTIFIER NOT NULL,
    PrescriptionDate DATE             NOT NULL,
    DoctorName       NVARCHAR(100) NOT NULL,
    Medications      NVARCHAR(MAX) NOT NULL,
    Dosage           NVARCHAR(MAX) NULL,
    Diagnosis        NVARCHAR(MAX) NULL,
    Duration         NVARCHAR(100) NULL,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 17.4 Vaccination Records
CREATE TABLE VaccinationRecords
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId        UNIQUEIDENTIFIER NOT NULL,
    VaccineName     NVARCHAR(200) NOT NULL,
    VaccinationDate DATE             NOT NULL,
    NextDoseDate    DATE NULL,
    DoseNumber      INT              NOT NULL,
    AdministeredBy  NVARCHAR(200) NULL,
    LotNumber       NVARCHAR(100) NULL,
    SideEffects     NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

-- 17.5 Occupational Health Records (İSG)
CREATE TABLE OccupationalHealthRecords
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId          UNIQUEIDENTIFIER NOT NULL,
    ExaminationType     NVARCHAR(100) NOT NULL, -- PeriodicCheckup, PreEmployment, etc.
    ExaminationDate     DATE             NOT NULL,
    NextExaminationDate DATE NULL,
    Result              NVARCHAR(100) NOT NULL, -- Fit, NotFit, Conditional
    IsFitForWork        BIT                          DEFAULT 1,
    Restrictions        NVARCHAR(MAX) NULL,
    DoctorName          NVARCHAR(200) NULL,
    HospitalName        NVARCHAR(200) NULL,
    CertificateNumber   NVARCHAR(100) NULL,
    Notes               NVARCHAR(MAX) NULL,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id)
);

-- 17.6 Work Accidents
CREATE TABLE WorkAccidents
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId         UNIQUEIDENTIFIER NOT NULL,
    AccidentNumber     NVARCHAR(50) NOT NULL UNIQUE,
    AccidentDate       DATETIME2        NOT NULL,
    Location           NVARCHAR(200) NOT NULL,
    Description        NVARCHAR(MAX) NOT NULL,
    InjuryType         NVARCHAR(200) NULL,
    BodyPart           NVARCHAR(200) NULL,
    Severity           INT              NOT NULL, -- 0=Minor, 1=Moderate, 2=Major, 3=Fatal
    DaysLost           INT                          DEFAULT 0,
    WitnessNames       NVARCHAR(MAX) NULL,
    TreatmentProvided  NVARCHAR(MAX) NULL,
    ReportedBy         UNIQUEIDENTIFIER NOT NULL,
    InvestigationNotes NVARCHAR(MAX) NULL,
    PreventiveMeasures NVARCHAR(MAX) NULL,
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    FOREIGN KEY (ReportedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 17: Health Services tables created.';
GO

-- ============================================================
-- MODULE 18: RESEARCH & PUBLICATIONS (ARAŞTIRMA VE YAYINLAR)
-- ============================================================
PRINT '📦 Module 18: Creating Research & Publications tables...';
GO

-- 18.1 Research Projects
CREATE TABLE ResearchProjects
(
    Id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectCode             NVARCHAR(50) NOT NULL UNIQUE,
    ProjectTitle            NVARCHAR(500) NOT NULL,
    ProjectType             INT              NOT NULL,              -- 0=Internal, 1=TUBITAK, 2=EU, 3=Industry, 4=International
    PrincipalInvestigatorId UNIQUEIDENTIFIER NOT NULL,
    DepartmentId            UNIQUEIDENTIFIER NOT NULL,
    StartDate               DATE             NOT NULL,
    EndDate                 DATE             NOT NULL,
    TotalBudget             DECIMAL(18, 4)   NOT NULL,
    SpentBudget             DECIMAL(18, 4)               DEFAULT 0,
    RemainingBudget AS (TotalBudget - SpentBudget) PERSISTED,
    FundingSource           NVARCHAR(200) NOT NULL,
    AbstractText            NVARCHAR(MAX) NULL,
    Keywords                NVARCHAR(MAX) NULL,
    Status                  INT                          DEFAULT 0, -- 0=Proposed, 1=Approved, 2=Active, 3=Completed, 4=Cancelled
    CreatedAt               DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PrincipalInvestigatorId) REFERENCES Persons (Id),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id)
);

-- 18.2 Project Team Members
CREATE TABLE ProjectTeamMembers
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectId          UNIQUEIDENTIFIER NOT NULL,
    StaffId            UNIQUEIDENTIFIER NOT NULL,
    Role               NVARCHAR(100) NOT NULL, -- PI, Co-PI, Researcher, RA, Technician
    StartDate          DATE             NOT NULL,
    EndDate            DATE NULL,
    WorkloadPercentage INT              NOT NULL    DEFAULT 0,
    Responsibilities   NVARCHAR(MAX) NULL,
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ProjectId) REFERENCES ResearchProjects (Id),
    FOREIGN KEY (StaffId) REFERENCES Persons (Id)
);

-- 18.3 Publications
CREATE TABLE Publications
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StaffId             UNIQUEIDENTIFIER NOT NULL,
    PublicationType     INT              NOT NULL, -- 0=Journal, 1=Conference, 2=Book, 3=BookChapter, 4=Patent
    Title               NVARCHAR(500) NOT NULL,
    Authors             NVARCHAR(MAX) NOT NULL,
    JournalOrConference NVARCHAR(300) NULL,
    Volume              NVARCHAR(50) NULL,
    Issue               NVARCHAR(50) NULL,
    Pages               NVARCHAR(50) NULL,
    Year                INT              NOT NULL,
    DOI                 NVARCHAR(200) NULL,
    ISSN                NVARCHAR(20) NULL,
    ISBN                NVARCHAR(20) NULL,
    ImpactFactor        DECIMAL(5, 2) NULL,
    CitationCount       INT                          DEFAULT 0,
    IsIndexed           BIT                          DEFAULT 0,
    IndexName           NVARCHAR(100) NULL,        -- SCI, SSCI, AHCI, Scopus, etc.
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StaffId) REFERENCES Persons (Id)
);

-- 18.4 Patents
CREATE TABLE Patents
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatentNumber    NVARCHAR(100) NOT NULL UNIQUE,
    PatentTitle     NVARCHAR(500) NOT NULL,
    Inventors       NVARCHAR(MAX) NOT NULL,
    ApplicationDate DATE NOT NULL,
    ApprovalDate    DATE NULL,
    Country         NVARCHAR(100) NOT NULL,
    PatentType      NVARCHAR(100) NULL,
    AbstractText    NVARCHAR(MAX) NULL,
    Status          INT                          DEFAULT 0, -- 0=Applied, 1=Pending, 2=Granted, 3=Rejected
    DocumentPath    NVARCHAR(500) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE()
);

-- 18.5 Project Expenses
CREATE TABLE ProjectExpenses
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProjectId     UNIQUEIDENTIFIER NOT NULL,
    ExpenseDate   DATE             NOT NULL,
    Category      NVARCHAR(100) NOT NULL,                 -- Equipment, Personnel, Travel, Supplies, etc.
    Amount        DECIMAL(18, 4)   NOT NULL,
    Description   NVARCHAR(MAX) NOT NULL,
    InvoiceNumber NVARCHAR(100) NULL,
    InvoicePath   NVARCHAR(500) NULL,
    ApprovedBy    UNIQUEIDENTIFIER NULL,
    ApprovalDate  DATETIME2 NULL,
    Status        INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Paid, 3=Rejected
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ProjectId) REFERENCES ResearchProjects (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 18: Research & Publications tables created.';
GO

-- ============================================================
-- MODULE 19: STUDENT ACTIVITIES (KULÜPLER VE ETKİNLİKLER)
-- ============================================================
PRINT '📦 Module 19: Creating Student Activities tables...';
GO

-- 19.1 Clubs
CREATE TABLE Clubs
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClubCode           NVARCHAR(20) NOT NULL UNIQUE,
    ClubName           NVARCHAR(200) NOT NULL,
    Description        NVARCHAR(MAX) NULL,
    EstablishedDate    DATE NOT NULL,
    AdvisorId          UNIQUEIDENTIFIER NULL,
    PresidentStudentId UNIQUEIDENTIFIER NULL,
    MemberCount        INT                          DEFAULT 0,
    BudgetAmount       DECIMAL(18, 4)               DEFAULT 0,
    Status             INT                          DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (AdvisorId) REFERENCES Persons (Id),
    FOREIGN KEY (PresidentStudentId) REFERENCES Students (Id)
);

-- 19.2 Club Members
CREATE TABLE ClubMembers
(
    Id        UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClubId    UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    JoinDate  DATE             NOT NULL,
    Role      NVARCHAR(100) NULL, -- President, VicePresident, Secretary, Treasurer, Member
    IsActive  BIT                          DEFAULT 1,
    CreatedAt DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClubId) REFERENCES Clubs (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id)
);

-- 19.3 Events
CREATE TABLE Events
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventCode            NVARCHAR(50) NOT NULL UNIQUE,
    EventName            NVARCHAR(300) NOT NULL,
    Description          NVARCHAR(MAX) NULL,
    EventType            INT       NOT NULL,                     -- 0=Academic, 1=Social, 2=Sports, 3=Cultural, 4=Career
    EventDate            DATETIME2 NOT NULL,
    EndDate              DATETIME2 NULL,
    Location             NVARCHAR(200) NULL,
    Capacity             INT NULL,
    CurrentRegistrations INT                          DEFAULT 0,
    OrganizerClubId      UNIQUEIDENTIFIER NULL,
    ContactPersonId      UNIQUEIDENTIFIER NULL,
    RequiresRegistration BIT                          DEFAULT 0,
    IsFree               BIT                          DEFAULT 1,
    Price                DECIMAL(18, 4)               DEFAULT 0,
    Status               INT                          DEFAULT 0, -- 0=Planned, 1=Active, 2=Completed, 3=Cancelled
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (OrganizerClubId) REFERENCES Clubs (Id),
    FOREIGN KEY (ContactPersonId) REFERENCES Persons (Id)
);

-- 19.4 Event Registrations
CREATE TABLE EventRegistrations
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId           UNIQUEIDENTIFIER NOT NULL,
    PersonId          UNIQUEIDENTIFIER NOT NULL,
    RegistrationDate  DATETIME2                    DEFAULT GETUTCDATE(),
    AttendanceStatus  INT                          DEFAULT 0, -- 0=Registered, 1=Attended, 2=NoShow, 3=Cancelled
    PaymentStatus     INT                          DEFAULT 0, -- 0=Unpaid, 1=Paid, 2=Refunded
    CertificateIssued BIT                          DEFAULT 0,
    CertificateNumber NVARCHAR(100) NULL,
    FeedbackRating    INT NULL,
    FeedbackComments  NVARCHAR(MAX) NULL,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EventId) REFERENCES Events (Id),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 19: Student Activities tables created.';
GO

-- ============================================================
-- MODULE 20: ANNOUNCEMENTS & SURVEYS (DUYURU VE ANKETLER)
-- ============================================================
PRINT '📦 Module 20: Creating Announcements & Surveys tables...';
GO

-- 20.1 Announcements
CREATE TABLE Announcements
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title            NVARCHAR(300) NOT NULL,
    Content          NVARCHAR(MAX) NOT NULL,
    AnnouncementType INT              NOT NULL,              -- 0=General, 1=Academic, 2=Administrative, 3=Urgent, 4=Emergency
    Priority         INT                          DEFAULT 1, -- 0=Low, 1=Normal, 2=High, 3=Critical
    TargetAudience   NVARCHAR(200) NOT NULL,                 -- Students, Staff, Everyone, Specific Role
    PublishDate      DATETIME2        NOT NULL,
    ExpiryDate       DATETIME2 NULL,
    CreatedBy        UNIQUEIDENTIFIER NOT NULL,
    ViewCount        INT                          DEFAULT 0,
    ImagePath        NVARCHAR(500) NULL,
    AttachmentPath   NVARCHAR(500) NULL,
    IsActive         BIT                          DEFAULT 1,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id)
);

-- 20.2 Surveys
CREATE TABLE Surveys
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SurveyTitle    NVARCHAR(300) NOT NULL,
    Description    NVARCHAR(MAX) NULL,
    StartDate      DATETIME2        NOT NULL,
    EndDate        DATETIME2        NOT NULL,
    TargetAudience NVARCHAR(200) NOT NULL,
    CreatedBy      UNIQUEIDENTIFIER NOT NULL,
    IsAnonymous    BIT                          DEFAULT 1,
    IsActive       BIT                          DEFAULT 1,
    ResponseCount  INT                          DEFAULT 0,
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id)
);

-- 20.3 Survey Questions
CREATE TABLE SurveyQuestions
(
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SurveyId      UNIQUEIDENTIFIER NOT NULL,
    QuestionText  NVARCHAR(MAX) NOT NULL,
    QuestionType  INT              NOT NULL, -- 0=MultipleChoice, 1=Text, 2=Rating, 3=YesNo, 4=Checkbox
    QuestionOrder INT              NOT NULL,
    IsRequired    BIT                          DEFAULT 0,
    Options       NVARCHAR(MAX) NULL,        -- JSON for multiple choice options
    CreatedAt     DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (SurveyId) REFERENCES Surveys (Id)
);

-- 20.4 Survey Responses
CREATE TABLE SurveyResponses
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SurveyId     UNIQUEIDENTIFIER NOT NULL,
    QuestionId   UNIQUEIDENTIFIER NOT NULL,
    RespondentId UNIQUEIDENTIFIER NULL, -- Null if anonymous
    ResponseText NVARCHAR(MAX) NULL,
    ResponseDate DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (SurveyId) REFERENCES Surveys (Id),
    FOREIGN KEY (QuestionId) REFERENCES SurveyQuestions (Id),
    FOREIGN KEY (RespondentId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 20: Announcements & Surveys tables created.';
GO

-- ============================================================
-- MODULE 21: DOCUMENT MANAGEMENT (BELGE YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 21: Creating Document Management tables...';
GO

-- 21.1 Document Requests (Belge Talepleri)
CREATE TABLE DocumentRequests
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequestNumber   NVARCHAR(50) NOT NULL UNIQUE,
    RequestDate     DATETIME2                    DEFAULT GETUTCDATE(),
    RequestedBy     UNIQUEIDENTIFIER NOT NULL,
    DocumentType    INT              NOT NULL,              -- 0=StudentCertificate, 1=Transcript, 2=Diploma, 3=ApprovalLetter
    Purpose         NVARCHAR(MAX) NULL,
    LanguageCode    NVARCHAR(20) DEFAULT 'TR',              -- TR, EN
    Quantity        INT                          DEFAULT 1,
    DeliveryMethod  INT                          DEFAULT 0, -- 0=Pickup, 1=Mail, 2=Digital
    DeliveryAddress NVARCHAR(MAX) NULL,
    Status          INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Processing, 3=Ready, 4=Delivered
    ApprovedBy      UNIQUEIDENTIFIER NULL,
    ApprovalDate    DATETIME2 NULL,
    ProcessedBy     UNIQUEIDENTIFIER NULL,
    CompletedDate   DATETIME2 NULL,
    Fee             DECIMAL(18, 4)               DEFAULT 0,
    PaymentStatus   INT                          DEFAULT 0,
    Notes           NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (RequestedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id),
    FOREIGN KEY (ProcessedBy) REFERENCES Persons (Id)
);

-- 21.2 Official Correspondence (Resmi Yazışmalar)
CREATE TABLE OfficialCorrespondence
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CorrespondenceNumber NVARCHAR(50) NOT NULL UNIQUE,
    CorrespondenceType   INT  NOT NULL,                          -- 0=Incoming, 1=Outgoing, 2=Internal
    Subject              NVARCHAR(500) NOT NULL,
    FromEntity           NVARCHAR(200) NOT NULL,
    ToEntity             NVARCHAR(200) NOT NULL,
    CorrespondenceDate   DATE NOT NULL,
    ReceivedDate         DATE NULL,
    Content              NVARCHAR(MAX) NULL,
    AttachmentCount      INT                          DEFAULT 0,
    Priority             INT                          DEFAULT 1, -- 0=Low, 1=Normal, 2=High, 3=Urgent
    Status               INT                          DEFAULT 0, -- 0=Draft, 1=Sent, 2=Received, 3=UnderReview, 4=Replied, 5=Archived
    AssignedTo           UNIQUEIDENTIFIER NULL,
    DueDate              DATE NULL,
    ResponseRequired     BIT                          DEFAULT 0,
    ResponseDate         DATE NULL,
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (AssignedTo) REFERENCES Persons (Id)
);

-- 21.3 Correspondence Attachments
CREATE TABLE CorrespondenceAttachments
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CorrespondenceId UNIQUEIDENTIFIER NOT NULL,
    FileName         NVARCHAR(300) NOT NULL,
    FilePath         NVARCHAR(500) NOT NULL,
    FileSize         BIGINT           NOT NULL,
    FileType         NVARCHAR(50) NULL,
    UploadDate       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CorrespondenceId) REFERENCES OfficialCorrespondence (Id)
);

PRINT
'✅ Module 21: Document Management tables created.';
GO

-- ============================================================
-- MODULE 22: SCHOLARSHIPS (BURS VE YARDIMLAR)
-- ============================================================
PRINT '📦 Module 22: Creating Scholarships tables...';
GO

-- 22.1 Scholarships
CREATE TABLE Scholarships
(
    Id                   UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScholarshipCode      NVARCHAR(50) NOT NULL UNIQUE,
    ScholarshipName      NVARCHAR(200) NOT NULL,
    Description          NVARCHAR(MAX) NULL,
    ScholarshipType      INT            NOT NULL, -- 0=Academic, 1=Financial, 2=Sports, 3=Art, 4=Merit
    Amount               DECIMAL(18, 4) NOT NULL,
    Currency             NVARCHAR(10) DEFAULT 'TRY',
    DurationSemesters    INT            NOT NULL,
    Criteria             NVARCHAR(MAX) NULL,
    MinimumGANO          DECIMAL(5, 2) NULL,
    MaximumFamilyIncome  DECIMAL(18, 4) NULL,
    AvailableSlots       INT            NOT NULL,
    UsedSlots            INT                          DEFAULT 0,
    ApplicationStartDate DATE           NOT NULL,
    ApplicationEndDate   DATE           NOT NULL,
    IsActive             BIT                          DEFAULT 1,
    CreatedAt            DATETIME2                    DEFAULT GETUTCDATE()
);

-- 22.2 Scholarship Applications
CREATE TABLE ScholarshipApplications
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ScholarshipId       UNIQUEIDENTIFIER NOT NULL,
    StudentId           UNIQUEIDENTIFIER NOT NULL,
    ApplicationDate     DATETIME2                    DEFAULT GETUTCDATE(),
    GANO                DECIMAL(5, 2) NULL,
    FamilyIncome        DECIMAL(18, 4) NULL,
    ApplicationLetter   NVARCHAR(MAX) NULL,
    SupportingDocuments NVARCHAR(MAX) NULL,
    Status              INT                          DEFAULT 0, -- 0=Pending, 1=UnderReview, 2=Approved, 3=Rejected
    ReviewedBy          UNIQUEIDENTIFIER NULL,
    ReviewDate          DATETIME2 NULL,
    ReviewComments      NVARCHAR(MAX) NULL,
    AwardedAmount       DECIMAL(18, 4) NULL,
    AwardedSemesters    INT NULL,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ScholarshipId) REFERENCES Scholarships (Id),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ReviewedBy) REFERENCES Persons (Id)
);

-- 22.3 Financial Aids (Sosyal Yardım)
CREATE TABLE FinancialAids
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StudentId           UNIQUEIDENTIFIER NOT NULL,
    AidType             NVARCHAR(100) NOT NULL,                 -- Emergency, Food, Books, Transportation, etc.
    Amount              DECIMAL(18, 4)   NOT NULL,
    AidDate             DATE             NOT NULL,
    Reason              NVARCHAR(MAX) NOT NULL,
    SupportingDocuments NVARCHAR(MAX) NULL,
    ApprovedBy          UNIQUEIDENTIFIER NULL,
    ApprovalDate        DATETIME2 NULL,
    Status              INT                          DEFAULT 0, -- 0=Pending, 1=Approved, 2=Disbursed, 3=Rejected
    DisbursementDate    DATE NULL,
    DisbursementMethod  NVARCHAR(100) NULL,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ApprovedBy) REFERENCES Persons (Id)
);

PRINT
'✅ Module 22: Scholarships tables created.';
GO

-- ============================================================
-- MODULE 23: PERFORMANCE MANAGEMENT (PERFORMANS YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 23: Creating Performance Management tables...';
GO

-- 23.1 Performance Reviews
CREATE TABLE PerformanceReviews
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId          UNIQUEIDENTIFIER NOT NULL,
    ReviewPeriodStart   DATE             NOT NULL,
    ReviewPeriodEnd     DATE             NOT NULL,
    ReviewDate          DATE             NOT NULL,
    ReviewerId          UNIQUEIDENTIFIER NOT NULL,
    OverallScore        DECIMAL(5, 2) NULL,
    Strengths           NVARCHAR(MAX) NULL,
    AreasForImprovement NVARCHAR(MAX) NULL,
    Goals               NVARCHAR(MAX) NULL,
    DevelopmentPlan     NVARCHAR(MAX) NULL,
    EmployeeComments    NVARCHAR(MAX) NULL,
    Status              INT                          DEFAULT 0, -- 0=Draft, 1=Submitted, 2=Reviewed, 3=Acknowledged
    AcknowledgedDate    DATE NULL,
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
    FOREIGN KEY (ReviewerId) REFERENCES Persons (Id)
);

-- 23.2 Performance Criteria
CREATE TABLE PerformanceCriteria
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CriteriaName NVARCHAR(200) NOT NULL,
    Description  NVARCHAR(MAX) NULL,
    Category     NVARCHAR(100) NOT NULL, -- Technical, Behavioral, Leadership
    Weight       DECIMAL(5, 2) NOT NULL, -- Percentage weight
    MaxScore     DECIMAL(5, 2)                DEFAULT 5.00,
    IsActive     BIT                          DEFAULT 1,
    CreatedAt    DATETIME2                    DEFAULT GETUTCDATE()
);

-- 23.3 Performance Scores
CREATE TABLE PerformanceScores
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReviewId   UNIQUEIDENTIFIER NOT NULL,
    CriteriaId UNIQUEIDENTIFIER NOT NULL,
    Score      DECIMAL(5, 2)    NOT NULL,
    Comments   NVARCHAR(MAX) NULL,
    CreatedAt  DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ReviewId) REFERENCES PerformanceReviews (Id),
    FOREIGN KEY (CriteriaId) REFERENCES PerformanceCriteria (Id)
);

-- 23.4 KPIs (Key Performance Indicators)
CREATE TABLE KPIs
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KPIName             NVARCHAR(200) NOT NULL,
    Description         NVARCHAR(MAX) NULL,
    UnitOfMeasure       NVARCHAR(50) NOT NULL,
    TargetValue         DECIMAL(18, 4) NOT NULL,
    ActualValue         DECIMAL(18, 4) NULL,
    MeasurementPeriod   NVARCHAR(50) NOT NULL,                  -- Monthly, Quarterly, Annual
    DepartmentId        UNIQUEIDENTIFIER NULL,
    ResponsiblePersonId UNIQUEIDENTIFIER NULL,
    Status              INT                          DEFAULT 0, -- 0=InProgress, 1=Achieved, 2=NotAchieved
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments (Id),
    FOREIGN KEY (ResponsiblePersonId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 23: Performance Management tables created.';
GO

-- ============================================================
-- MODULE 24: GRADUATION & DIPLOMA (MEZUNİYET VE DİPLOMA)
-- ============================================================
PRINT '📦 Module 24: Creating Graduation & Diploma tables...';
GO

-- 24.1 Graduation Requirements
CREATE TABLE GraduationRequirements
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramId          UNIQUEIDENTIFIER NOT NULL,
    MinimumGANO        DECIMAL(5, 2)    NOT NULL    DEFAULT 2.00,
    MinimumCredits     INT              NOT NULL,
    MaxAllowedFailures INT                          DEFAULT 0,
    MaximumYears       INT              NOT NULL,
    RequiredCourses    NVARCHAR(MAX) NULL,
    OtherRequirements  NVARCHAR(MAX) NULL,
    EffectiveDate      DATE             NOT NULL,
    CreatedAt          DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ProgramId) REFERENCES Programs (Id)
);

-- 24.2 Diplomas
CREATE TABLE Diplomas
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DiplomaNumber  NVARCHAR(100) NOT NULL UNIQUE,
    StudentId      UNIQUEIDENTIFIER NOT NULL,
    ProgramId      UNIQUEIDENTIFIER NOT NULL,
    GraduationDate DATE             NOT NULL,
    GANO           DECIMAL(5, 2)    NOT NULL,
    Honors         NVARCHAR(100) NULL,                     -- HighHonors, Honors, None
    DegreeTitle    NVARCHAR(200) NOT NULL,
    IssueDate      DATE             NOT NULL,
    IssuedBy       UNIQUEIDENTIFIER NOT NULL,
    Status         INT                          DEFAULT 0, -- 0=Pending, 1=Issued, 2=Delivered, 3=Reissued
    DeliveryDate   DATE NULL,
    DeliveryMethod INT NULL,                               -- 0=InPerson, 1=Mail, 2=Courier
    Notes          NVARCHAR(MAX) NULL,
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (StudentId) REFERENCES Students (Id),
    FOREIGN KEY (ProgramId) REFERENCES Programs (Id),
    FOREIGN KEY (IssuedBy) REFERENCES Persons (Id)
);

-- 24.3 Apostilles
CREATE TABLE Apostilles
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DiplomaId        UNIQUEIDENTIFIER NOT NULL,
    RequestDate      DATE             NOT NULL,
    TargetCountry    NVARCHAR(100) NOT NULL,
    ApostilleNumber  NVARCHAR(100) NULL,
    IssueDate        DATE NULL,
    IssuingAuthority NVARCHAR(200) NULL,
    Status           INT                          DEFAULT 0, -- 0=Requested, 1=InProgress, 2=Completed, 3=Rejected
    CompletionDate   DATE NULL,
    Cost             DECIMAL(18, 4) NULL,
    DeliveryDate     DATE NULL,
    Notes            NVARCHAR(MAX) NULL,
    CreatedAt        DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (DiplomaId) REFERENCES Diplomas (Id)
);

PRINT
'✅ Module 24: Graduation & Diploma tables created.';
GO

-- ============================================================
-- MODULE 25: TECHNICAL SERVICES (TEKNİK SERVİS VE BAKIM)
-- ============================================================
PRINT '📦 Module 25: Creating Technical Services tables...';
GO

-- 25.1 Maintenance Requests
CREATE TABLE MaintenanceRequests
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RequestNumber  NVARCHAR(50) NOT NULL UNIQUE,
    RequestDate    DATETIME2                    DEFAULT GETUTCDATE(),
    RequestedBy    UNIQUEIDENTIFIER NOT NULL,
    Location       NVARCHAR(200) NOT NULL,
    BuildingId     UNIQUEIDENTIFIER NULL,
    RoomId         UNIQUEIDENTIFIER NULL,
    IssueType      NVARCHAR(100) NOT NULL,                 -- Electrical, Plumbing, HVAC, IT, etc.
    Description    NVARCHAR(MAX) NOT NULL,
    Priority       INT                          DEFAULT 1, -- 0=Low, 1=Medium, 2=High, 3=Critical
    Status         INT                          DEFAULT 0, -- 0=New, 1=Assigned, 2=InProgress, 3=OnHold, 4=Completed, 5=Cancelled
    AssignedTo     UNIQUEIDENTIFIER NULL,
    AssignmentDate DATETIME2 NULL,
    StartDate      DATETIME2 NULL,
    CompletedDate  DATETIME2 NULL,
    Resolution     NVARCHAR(MAX) NULL,
    Cost           DECIMAL(18, 4) NULL,
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (RequestedBy) REFERENCES Persons (Id),
    FOREIGN KEY (BuildingId) REFERENCES Buildings (Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms (Id),
    FOREIGN KEY (AssignedTo) REFERENCES Persons (Id)
);

-- 25.2 Maintenance Schedule
CREATE TABLE MaintenanceSchedules
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EquipmentId         NVARCHAR(100) NOT NULL,
    EquipmentName       NVARCHAR(200) NOT NULL,
    EquipmentType       NVARCHAR(100) NOT NULL,
    Location            NVARCHAR(200) NOT NULL,
    MaintenanceType     NVARCHAR(100) NOT NULL,                 -- Preventive, Corrective, Inspection
    Frequency           NVARCHAR(50) NOT NULL,                  -- Daily, Weekly, Monthly, Quarterly, Annual
    LastMaintenanceDate DATE NULL,
    NextMaintenanceDate DATE NOT NULL,
    ResponsiblePerson   UNIQUEIDENTIFIER NULL,
    EstimatedDuration   INT NULL,                               -- in hours
    Checklist           NVARCHAR(MAX) NULL,
    Status              INT                          DEFAULT 0, -- 0=Scheduled, 1=Completed, 2=Skipped, 3=Rescheduled
    CreatedAt           DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (ResponsiblePerson) REFERENCES Persons (Id)
);

-- 25.3 Periodic Maintenance Plans
CREATE TABLE PeriodicMaintenancePlans
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PlanName          NVARCHAR(200) NOT NULL,
    Description       NVARCHAR(MAX) NULL,
    Frequency         NVARCHAR(50) NOT NULL,
    DurationMinutes   INT NOT NULL,
    Checklist         NVARCHAR(MAX) NULL,
    RequiredTools     NVARCHAR(MAX) NULL,
    RequiredMaterials NVARCHAR(MAX) NULL,
    Status            INT                          DEFAULT 0,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE()
);

-- 25.4 Energy Consumption
CREATE TABLE EnergyConsumption
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReadingDate     DATE NOT NULL,
    BuildingId      UNIQUEIDENTIFIER NULL,
    BuildingName    NVARCHAR(100) NOT NULL,
    ElectricityKWh  DECIMAL(10, 2)               DEFAULT 0,
    WaterM3         DECIMAL(10, 2)               DEFAULT 0,
    NaturalGasM3    DECIMAL(10, 2)               DEFAULT 0,
    ElectricityCost DECIMAL(18, 4)               DEFAULT 0,
    WaterCost       DECIMAL(18, 4)               DEFAULT 0,
    GasCost         DECIMAL(18, 4)               DEFAULT 0,
    TotalCost       DECIMAL(18, 4)               DEFAULT 0,
    Notes           NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (BuildingId) REFERENCES Buildings (Id)
);

PRINT
'✅ Module 25: Technical Services tables created.';
GO

-- ============================================================
-- MODULE 26: IT MANAGEMENT (BİLGİ TEKNOLOJİLERİ YÖNETİMİ)
-- ============================================================
PRINT '📦 Module 26: Creating IT Management tables...';
GO

-- 26.1 Servers
CREATE TABLE Servers
(
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ServerName      NVARCHAR(100) NOT NULL UNIQUE,
    IPAddress       NVARCHAR(50) NOT NULL,
    ServerType      NVARCHAR(100) NOT NULL,                 -- Web, Database, Application, Mail, etc.
    OperatingSystem NVARCHAR(100) NOT NULL,
    CPU             NVARCHAR(100) NULL,
    RAM             NVARCHAR(50) NULL,
    Storage         NVARCHAR(50) NULL,
    Location        NVARCHAR(200) NULL,
    PurchaseDate    DATE NULL,
    WarrantyExpiry  DATE NULL,
    Status          INT                          DEFAULT 0, -- 0=Active, 1=Inactive, 2=Maintenance, 3=Decommissioned
    CreatedAt       DATETIME2                    DEFAULT GETUTCDATE()
);

-- 26.2 Network Devices
CREATE TABLE NetworkDevices
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DeviceName     NVARCHAR(100) NOT NULL,
    DeviceType     NVARCHAR(50) NOT NULL, -- Router, Switch, Firewall, AccessPoint, etc.
    IPAddress      NVARCHAR(50) NULL,
    MACAddress     NVARCHAR(50) NULL,
    Location       NVARCHAR(200) NULL,
    Model          NVARCHAR(100) NULL,
    SerialNumber   NVARCHAR(100) NULL,
    PurchaseDate   DATE NULL,
    WarrantyExpiry DATE NULL,
    Firmware       NVARCHAR(100) NULL,
    Status         INT                          DEFAULT 0,
    CreatedAt      DATETIME2                    DEFAULT GETUTCDATE()
);

-- 26.3 Software Licenses
CREATE TABLE SoftwareLicenses
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SoftwareName      NVARCHAR(200) NOT NULL,
    Version           NVARCHAR(50) NULL,
    LicenseKey        NVARCHAR(MAX) NOT NULL,
    LicenseType       NVARCHAR(100) NOT NULL, -- Perpetual, Subscription, Volume
    PurchaseDate      DATE NOT NULL,
    ExpiryDate        DATE NULL,
    TotalLicenses     INT  NOT NULL,
    UsedLicenses      INT                          DEFAULT 0,
    AvailableLicenses AS (TotalLicenses - UsedLicenses) PERSISTED,
    Cost              DECIMAL(18, 4) NULL,
    Vendor            NVARCHAR(200) NULL,
    SupportExpiryDate DATE NULL,
    Status            INT                          DEFAULT 0,
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE()
);

-- 26.4 Help Desk Tickets
CREATE TABLE HelpDeskTickets
(
    Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TicketNumber      NVARCHAR(50) NOT NULL UNIQUE,
    CreatedBy         UNIQUEIDENTIFIER NOT NULL,
    Category          NVARCHAR(100) NOT NULL,                 -- Hardware, Software, Network, Account, etc.
    SubCategory       NVARCHAR(100) NULL,
    Priority          INT                          DEFAULT 1,
    Subject           NVARCHAR(300) NOT NULL,
    Description       NVARCHAR(MAX) NOT NULL,
    Status            INT                          DEFAULT 0, -- 0=New, 1=Open, 2=InProgress, 3=Pending, 4=Resolved, 5=Closed
    AssignedTo        UNIQUEIDENTIFIER NULL,
    AssignmentDate    DATETIME2 NULL,
    FirstResponseDate DATETIME2 NULL,
    ResolvedDate      DATETIME2 NULL,
    ClosedDate        DATETIME2 NULL,
    Resolution        NVARCHAR(MAX) NULL,
    UserSatisfaction  INT NULL,                               -- 1-5 rating
    CreatedAt         DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (CreatedBy) REFERENCES Persons (Id),
    FOREIGN KEY (AssignedTo) REFERENCES Persons (Id)
);

-- 26.5 User Accounts (System Users)
CREATE TABLE UserAccounts
(
    Id                     UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId               UNIQUEIDENTIFIER NOT NULL,
    Username               NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash           NVARCHAR(MAX) NOT NULL,
    Email                  NVARCHAR(100) NOT NULL,
    LastLoginDate          DATETIME2 NULL,
    LastPasswordChangeDate DATETIME2 NULL,
    PasswordExpiryDate     DATETIME2 NULL,
    FailedLoginAttempts    INT                          DEFAULT 0,
    IsLocked               BIT                          DEFAULT 0,
    LockedUntil            DATETIME2 NULL,
    IsActive               BIT                          DEFAULT 1,
    TwoFactorEnabled       BIT                          DEFAULT 0,
    CreatedAt              DATETIME2                    DEFAULT GETUTCDATE(),
    UpdatedAt              DATETIME2                    DEFAULT GETUTCDATE(),
    FOREIGN KEY (PersonId) REFERENCES Persons (Id)
);

PRINT
'✅ Module 26: IT Management tables created.';
GO

-- ============================================================
-- ADDITIONAL SUPPORT TABLES
-- ============================================================
PRINT '📦 Creating additional support tables...';
GO

-- Messaging System
CREATE TABLE Messages
(
    Id             UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SenderId       UNIQUEIDENTIFIER NOT NULL,
    Subject        NVARCHAR(300) NOT NULL,
    MessageBody    NVARCHAR(MAX) NOT NULL,
    SentDate       DATETIME2                    DEFAULT GETUTCDATE(),
    Priority       INT                          DEFAULT 1,
    HasAttachments BIT                          DEFAULT 0,
    IsDeleted      BIT                          DEFAULT 0,
    FOREIGN KEY (SenderId) REFERENCES Persons (Id)
);

CREATE TABLE MessageRecipients
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MessageId   UNIQUEIDENTIFIER NOT NULL,
    RecipientId UNIQUEIDENTIFIER NOT NULL,
    IsRead      BIT                          DEFAULT 0,
    ReadDate    DATETIME2 NULL,
    IsFlagged   BIT                          DEFAULT 0,
    IsArchived  BIT                          DEFAULT 0,
    FOREIGN KEY (MessageId) REFERENCES Messages (Id),
    FOREIGN KEY (RecipientId) REFERENCES Persons (Id)
);

-- Workflow System
CREATE TABLE WorkflowApprovals
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WorkflowType     NVARCHAR(100) NOT NULL,
    EntityId         UNIQUEIDENTIFIER NOT NULL,
    CurrentStep      INT              NOT NULL,
    TotalSteps       INT              NOT NULL,
    ApproverPersonId UNIQUEIDENTIFIER NOT NULL,
    Status           INT                          DEFAULT 0,
    RequestDate      DATETIME2                    DEFAULT GETUTCDATE(),
    ResponseDate     DATETIME2 NULL,
    Comments         NVARCHAR(MAX) NULL,
    FOREIGN KEY (ApproverPersonId) REFERENCES Persons (Id)
);

PRINT
'✅ Additional support tables created.';
GO

-- ============================================================
-- CREATE INDEXES FOR PERFORMANCE OPTIMIZATION
-- ============================================================
PRINT '📊 Creating indexes for performance optimization...';
GO

-- Core Infrastructure Indexes
CREATE INDEX IX_Persons_IdentityNumber ON Persons (IdentityNumber);
CREATE INDEX IX_Persons_Email ON Persons (Email);
CREATE INDEX IX_Persons_PersonType ON Persons (PersonType);
CREATE INDEX IX_Persons_Status ON Persons (Status);

CREATE INDEX IX_Addresses_PersonId ON Addresses (PersonId);
CREATE INDEX IX_UserRoles_PersonId ON UserRoles (PersonId);
CREATE INDEX IX_UserRoles_RoleId ON UserRoles (RoleId);

-- Academic Indexes
CREATE INDEX IX_Students_StudentNumber ON Students (StudentNumber);
CREATE INDEX IX_Students_PersonId ON Students (PersonId);
CREATE INDEX IX_Students_ProgramId ON Students (ProgramId);
CREATE INDEX IX_Students_AdvisorId ON Students (AdvisorId);
CREATE INDEX IX_Students_RegistrationStatus ON Students (RegistrationStatus);

CREATE INDEX IX_Staff_StaffNumber ON Staff (StaffNumber);
CREATE INDEX IX_Staff_PersonId ON Staff (PersonId);
CREATE INDEX IX_Staff_DepartmentId ON Staff (DepartmentId);

CREATE INDEX IX_Courses_CourseCode ON Courses (CourseCode);
CREATE INDEX IX_Courses_DepartmentId ON Courses (DepartmentId);

CREATE INDEX IX_CourseRegistrations_StudentId ON CourseRegistrations (StudentId);
CREATE INDEX IX_CourseRegistrations_CourseScheduleId ON CourseRegistrations (CourseScheduleId);
CREATE INDEX IX_CourseRegistrations_SemesterId ON CourseRegistrations (SemesterId);

CREATE INDEX IX_Grades_StudentId ON Grades (StudentId);
CREATE INDEX IX_Grades_CourseRegistrationId ON Grades (CourseRegistrationId);

CREATE INDEX IX_Attendance_StudentId ON Attendance (StudentId);
CREATE INDEX IX_Attendance_CourseRegistrationId ON Attendance (CourseRegistrationId);
CREATE INDEX IX_Attendance_AttendanceDate ON Attendance (AttendanceDate);

-- Financial Indexes
CREATE INDEX IX_WalletTransactionHistory_WalletId ON WalletTransactionHistory (WalletId);
CREATE INDEX IX_WalletTransactionHistory_TransactionDate ON WalletTransactionHistory (TransactionDate);

CREATE INDEX IX_StudentFees_StudentId ON StudentFees (StudentId);
CREATE INDEX IX_StudentFees_SemesterId ON StudentFees (SemesterId);
CREATE INDEX IX_StudentFees_Status ON StudentFees (Status);

CREATE INDEX IX_PayrollDetails_EmployeeId ON PayrollDetails (EmployeeId);
CREATE INDEX IX_PayrollDetails_PayrollRunId ON PayrollDetails (PayrollRunId);

-- Security Indexes
CREATE INDEX IX_AccessLogs_PersonId ON AccessLogs (PersonId);
CREATE INDEX IX_AccessLogs_AccessPointId ON AccessLogs (AccessPointId);
CREATE INDEX IX_AccessLogs_AccessTime ON AccessLogs (AccessTime);

CREATE INDEX IX_AccessCards_PersonId ON AccessCards (PersonId);
CREATE INDEX IX_AccessCards_CardNumber ON AccessCards (CardNumber);
CREATE INDEX IX_AccessCards_QRCode ON AccessCards (QRCode);

-- Parking Indexes
CREATE INDEX IX_VehicleRegistration_PersonId ON VehicleRegistration (PersonId);
CREATE INDEX IX_VehicleRegistration_LicensePlate ON VehicleRegistration (LicensePlate);

CREATE INDEX IX_ParkingEntryExitLog_VehicleRegistrationId ON ParkingEntryExitLog (VehicleRegistrationId);
CREATE INDEX IX_ParkingEntryExitLog_EntryTime ON ParkingEntryExitLog (EntryTime);

-- Library Indexes
CREATE INDEX IX_Books_ISBN ON Books (ISBN);
CREATE INDEX IX_Books_CategoryId ON Books (CategoryId);

CREATE INDEX IX_Loans_BookId ON Loans (BookId);
CREATE INDEX IX_Loans_PersonId ON Loans (PersonId);
CREATE INDEX IX_Loans_Status ON Loans (Status);

CREATE INDEX IX_LibraryFines_PersonId ON LibraryFines (PersonId);
CREATE INDEX IX_LibraryFines_Status ON LibraryFines (Status);

-- Event Indexes
CREATE INDEX IX_Events_EventDate ON Events (EventDate);
CREATE INDEX IX_Events_Status ON Events (Status);

CREATE INDEX IX_EventRegistrations_EventId ON EventRegistrations (EventId);
CREATE INDEX IX_EventRegistrations_PersonId ON EventRegistrations (PersonId);

-- Help Desk Indexes
CREATE INDEX IX_HelpDeskTickets_TicketNumber ON HelpDeskTickets (TicketNumber);
CREATE INDEX IX_HelpDeskTickets_CreatedBy ON HelpDeskTickets (CreatedBy);
CREATE INDEX IX_HelpDeskTickets_AssignedTo ON HelpDeskTickets (AssignedTo);
CREATE INDEX IX_HelpDeskTickets_Status ON HelpDeskTickets (Status);

-- Audit Indexes
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs (UserId);
CREATE INDEX IX_AuditLogs_EntityName ON AuditLogs (EntityName);
CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs (Timestamp);

PRINT
'✅ All indexes created successfully.';
GO

-- ============================================================
-- FINAL MESSAGE
-- ============================================================
PRINT '';
PRINT
'═══════════════════════════════════════════════════════════';
PRINT
'✨ UNIVERSITY MANAGEMENT SYSTEM DATABASE v3.0 ✨';
PRINT
'═══════════════════════════════════════════════════════════';
PRINT
'✅ Installation completed successfully!';
PRINT
'';
PRINT
'📊 Database Statistics:';
PRINT
'   - Total Modules: 26 Main Modules';
PRINT
'   - Total Tables: 150+ Tables';
PRINT
'   - Total Indexes: 50+ Performance Indexes';
PRINT
'';
PRINT
'🎯 Key Features:';
PRINT
'   ✓ Complete Academic Management';
PRINT
'   ✓ HR & Payroll System';
PRINT
'   ✓ Financial Management';
PRINT
'   ✓ Procurement & Tenders';
PRINT
'   ✓ Inventory Management';
PRINT
'   ✓ Library Management';
PRINT
'   ✓ Security & Access Control';
PRINT
'   ✓ Parking Management';
PRINT
'   ✓ Cafeteria & Catering';
PRINT
'   ✓ Facility & Lab Management';
PRINT
'   ✓ Health Services';
PRINT
'   ✓ Research & Publications';
PRINT
'   ✓ Student Activities';
PRINT
'   ✓ IT Management';
PRINT
'   ✓ Performance Management';
PRINT
'   ✓ Graduation & Diploma';
PRINT
'   ✓ Technical Services';
PRINT
'';
PRINT
'🚀 Ready to start development!';
PRINT
'═══════════════════════════════════════════════════════════';
GO