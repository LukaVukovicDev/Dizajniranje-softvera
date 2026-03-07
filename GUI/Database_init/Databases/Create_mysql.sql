-- ============================================================
-- create_mysql.sql - Co-working sistem
-- Kompatibilno sa: MySQL 8+
-- ============================================================

-- Admins
CREATE TABLE IF NOT EXISTS Admins (
    Id           INT PRIMARY KEY AUTO_INCREMENT,
    Username     VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    CreatedAt    DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- MembershipTypes
CREATE TABLE IF NOT EXISTS MembershipTypes (
    Id                          INT PRIMARY KEY AUTO_INCREMENT,
    Name                        VARCHAR(100) NOT NULL,
    Price                       DECIMAL(10,2) NOT NULL,
    DurationDays                INT NOT NULL,
    MaxReservationHoursPerMonth INT NOT NULL DEFAULT 0,
    IncludesMeetingRooms        TINYINT(1) NOT NULL DEFAULT 0,
    MeetingRoomHoursPerMonth    INT NOT NULL DEFAULT 0,
    Description                 TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Locations
CREATE TABLE IF NOT EXISTS Locations (
    Id           INT PRIMARY KEY AUTO_INCREMENT,
    Name         VARCHAR(150) NOT NULL,
    Address      VARCHAR(255) NOT NULL,
    City         VARCHAR(100) NOT NULL,
    WorkingHours VARCHAR(100),
    MaxCapacity  INT NOT NULL DEFAULT 0,
    Description  TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Users
CREATE TABLE IF NOT EXISTS Users (
    Id                  INT PRIMARY KEY AUTO_INCREMENT,
    FirstName           VARCHAR(100) NOT NULL,
    LastName            VARCHAR(100) NOT NULL,
    Email               VARCHAR(200) NOT NULL UNIQUE,
    Phone               VARCHAR(30),
    MembershipTypeId    INT NOT NULL,
    MembershipStartDate DATE NOT NULL,
    MembershipEndDate   DATE NOT NULL,
    Status              ENUM('Active','Paused','Expired') NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_Users_MembershipType FOREIGN KEY (MembershipTypeId)
        REFERENCES MembershipTypes(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Resources
CREATE TABLE IF NOT EXISTS Resources (
    Id                        INT PRIMARY KEY AUTO_INCREMENT,
    LocationId                INT NOT NULL,
    Name                      VARCHAR(150) NOT NULL,
    Type                      ENUM('HotDesk','DedicatedDesk','PrivateOffice','MeetingRoom') NOT NULL,
    Description               TEXT,
    IsAvailable               TINYINT(1) NOT NULL DEFAULT 1,
    Capacity                  INT,
    HasProjector              TINYINT(1) DEFAULT 0,
    HasTV                     TINYINT(1) DEFAULT 0,
    HasWhiteboard             TINYINT(1) DEFAULT 0,
    HasOnlineMeetingEquipment TINYINT(1) DEFAULT 0,
    SubType                   VARCHAR(20),
    CONSTRAINT FK_Resources_Location FOREIGN KEY (LocationId)
        REFERENCES Locations(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Reservations
CREATE TABLE IF NOT EXISTS Reservations (
    Id            INT PRIMARY KEY AUTO_INCREMENT,
    UserId        INT NOT NULL,
    ResourceId    INT NOT NULL,
    StartDateTime DATETIME NOT NULL,
    EndDateTime   DATETIME NOT NULL,
    Status        ENUM('Active','Completed','Cancelled') NOT NULL DEFAULT 'Active',
    CreatedAt     DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Reservations_User FOREIGN KEY (UserId)
        REFERENCES Users(Id),
    CONSTRAINT FK_Reservations_Resource FOREIGN KEY (ResourceId)
        REFERENCES Resources(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Indeksi
CREATE INDEX IDX_Reservations_StartEnd ON Reservations(StartDateTime, EndDateTime);
CREATE INDEX IDX_Reservations_User     ON Reservations(UserId);
CREATE INDEX IDX_Resources_Location    ON Resources(LocationId);
