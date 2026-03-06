-- ============================================================
-- create_mssql.sql - Co-working sistem
-- Kompatibilno sa: Microsoft SQL Server (MSSQL)
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CoWorkingDB')
    CREATE DATABASE CoWorkingDB;
GO
USE CoWorkingDB;
GO

-- Obrisi stare tabele (redosled zbog FK constraints)
IF OBJECT_ID('Reservations',    'U') IS NOT NULL DROP TABLE Reservations;
IF OBJECT_ID('Resources',       'U') IS NOT NULL DROP TABLE Resources;
IF OBJECT_ID('Users',           'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Locations',       'U') IS NOT NULL DROP TABLE Locations;
IF OBJECT_ID('MembershipTypes', 'U') IS NOT NULL DROP TABLE MembershipTypes;
IF OBJECT_ID('Admins',          'U') IS NOT NULL DROP TABLE Admins;
GO

-- Admins
CREATE TABLE Admins (
    AdminID      INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    CreatedAt    DATETIME2 DEFAULT GETDATE()
);
GO

-- MembershipTypes
CREATE TABLE MembershipTypes (
    ID                          INT IDENTITY(1,1) PRIMARY KEY,
    Name                        NVARCHAR(100) NOT NULL,
    Price                       DECIMAL(10,2) NOT NULL,
    DurationDays                INT NOT NULL,
    MaxReservationHoursPerMonth INT NOT NULL DEFAULT 0,
    IncludesMeetingRooms        BIT NOT NULL DEFAULT 0,
    MeetingRoomHoursMonth       INT NOT NULL DEFAULT 0,
    Description                 NVARCHAR(MAX)
);
GO

-- Locations
CREATE TABLE Locations (
    ID           INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(150) NOT NULL,
    Address      NVARCHAR(255) NOT NULL,
    City         NVARCHAR(100) NOT NULL,
    WorkingHours NVARCHAR(100),
    MaxCapacity  INT NOT NULL DEFAULT 0,
    Description  NVARCHAR(MAX)
);
GO

-- Users
CREATE TABLE Users (
    ID                  INT IDENTITY(1,1) PRIMARY KEY,
    FirstName           NVARCHAR(100) NOT NULL,
    LastName            NVARCHAR(100) NOT NULL,
    Email               NVARCHAR(200) NOT NULL UNIQUE,
    Phone               NVARCHAR(30),
    MembershipTypeID    INT NOT NULL,
    MembershipStartDate DATE NOT NULL,
    MembershipEndDate   DATE NOT NULL,
    Status              NVARCHAR(10) NOT NULL DEFAULT 'active'
        CHECK (Status IN ('active','paused','expired')),
    CONSTRAINT FK_Users_MembershipType FOREIGN KEY (MembershipTypeID)
        REFERENCES MembershipTypes(ID)
);
GO

-- Resources
CREATE TABLE Resources (
    ID                 INT IDENTITY(1,1) PRIMARY KEY,
    LocationID         INT NOT NULL,
    Name               NVARCHAR(150) NOT NULL,
    ResourceType       NVARCHAR(20) NOT NULL
        CHECK (ResourceType IN ('hot_desk','dedicated_desk','private_office','meeting_room')),
    Description        NVARCHAR(MAX),
    IsAvailable        BIT NOT NULL DEFAULT 1,
    Capacity           INT,
    HasProjector       BIT DEFAULT 0,
    HasTV              BIT DEFAULT 0,
    HasWhiteboard      BIT DEFAULT 0,
    HasOnlineEquipment BIT DEFAULT 0,
    CONSTRAINT FK_Resources_Location FOREIGN KEY (LocationID)
        REFERENCES Locations(ID)
);
GO

-- Reservations
CREATE TABLE Reservations (
    ID            INT IDENTITY(1,1) PRIMARY KEY,
    UserID        INT NOT NULL,
    ResourceID    INT NOT NULL,
    StartDateTime DATETIME2 NOT NULL,
    EndDateTime   DATETIME2 NOT NULL,
    Status        NVARCHAR(10) NOT NULL DEFAULT 'active'
        CHECK (Status IN ('active','completed','cancelled')),
    CreatedAt     DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Reservations_User FOREIGN KEY (UserID)
        REFERENCES Users(ID),
    CONSTRAINT FK_Reservations_Resource FOREIGN KEY (ResourceID)
        REFERENCES Resources(ID)
);
GO

-- Indeksi
CREATE INDEX IDX_Reservations_StartEnd ON Reservations(StartDateTime, EndDateTime);
CREATE INDEX IDX_Reservations_User     ON Reservations(UserID);
CREATE INDEX IDX_Resources_Location    ON Resources(LocationID);
GO