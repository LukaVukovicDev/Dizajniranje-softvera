-- ============================================================
-- schema_mysql.sql - Co-working sistem
-- Kompatibilno sa: MySQL 8+
-- ============================================================

-- Administratori (login)
CREATE TABLE IF NOT EXISTS Admins (
    AdminID      INT PRIMARY KEY AUTO_INCREMENT,
    Username     VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    CreatedAt    DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tipovi clanstva
CREATE TABLE IF NOT EXISTS MembershipTypes (
    ID         INT PRIMARY KEY AUTO_INCREMENT,
    Name                     VARCHAR(100) NOT NULL,
    Price                    DECIMAL(10,2) NOT NULL,
    DurationDays             INT NOT NULL,
    MaxReservationHoursPerMonth INT NOT NULL DEFAULT 0,
    IncludesMeetingRooms        TINYINT(1) NOT NULL DEFAULT 0,
    MeetingRoomHoursMonth    INT NOT NULL DEFAULT 0,
    Description              TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Lokacije
CREATE TABLE IF NOT EXISTS Locations (
    ID   INT PRIMARY KEY AUTO_INCREMENT,
    Name         VARCHAR(150) NOT NULL,
    Address      VARCHAR(255) NOT NULL,
    City         VARCHAR(100) NOT NULL,
    WorkingHours VARCHAR(100),
    MaxCapacity  INT NOT NULL DEFAULT 0,
    Description  TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Korisnici
CREATE TABLE IF NOT EXISTS Users (
    ID           INT PRIMARY KEY AUTO_INCREMENT,
    FirstName        VARCHAR(100) NOT NULL,
    LastName         VARCHAR(100) NOT NULL,
    Email            VARCHAR(200) NOT NULL UNIQUE,
    Phone            VARCHAR(30),
    MembershipTypeID INT NOT NULL,
    MembershipStartDate  DATE NOT NULL,
    MembershipEndDate    DATE NOT NULL,
    Status    ENUM('active','paused','expired') NOT NULL DEFAULT 'active',
    CONSTRAINT FK_Users_MembershipType FOREIGN KEY (MembershipTypeID)
        REFERENCES MembershipTypes(ID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Resursi (radna mesta i sale)
CREATE TABLE IF NOT EXISTS Resources (
    ID     INT PRIMARY KEY AUTO_INCREMENT,
    LocationID     INT NOT NULL,
    Name           VARCHAR(150) NOT NULL,
    ResourceType   ENUM('hot_desk','dedicated_desk','private_office','meeting_room') NOT NULL,
    Description    TEXT,
    IsAvailable    TINYINT(1) NOT NULL DEFAULT 1,
    -- Samo za meeting_room
    Capacity       INT,
    HasProjector   TINYINT(1) DEFAULT 0,
    HasTV          TINYINT(1) DEFAULT 0,
    HasWhiteboard  TINYINT(1) DEFAULT 0,
    HasOnlineEquipment TINYINT(1) DEFAULT 0,
    CONSTRAINT FK_Resources_Location FOREIGN KEY (LocationID)
        REFERENCES Locations(ID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Rezervacije
CREATE TABLE IF NOT EXISTS Reservations (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    UserID        INT NOT NULL,
    ResourceID    INT NOT NULL,
    StartDateTime DATETIME NOT NULL,
    EndDateTime   DATETIME NOT NULL,
    Status        ENUM('active','completed','cancelled') NOT NULL DEFAULT 'active',
    CreatedAt     DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Reservations_User FOREIGN KEY (UserID)
        REFERENCES Users(ID),
    CONSTRAINT FK_Reservations_Resource FOREIGN KEY (ResourceID)
        REFERENCES Resources(ID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Indeksi
CREATE INDEX IF NOT EXISTS IDX_Reservations_StartEnd ON Reservations(StartDateTime, EndDateTime);
CREATE INDEX IF NOT EXISTS IDX_Reservations_User     ON Reservations(UserID);
CREATE INDEX IF NOT EXISTS IDX_Resources_Location    ON Resources(LocationID);