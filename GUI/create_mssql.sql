

-- Kreiranje baze
CREATE DATABASE coworking;
GO

USE coworking;
GO

-- =========================
-- TIPOVI CLANSTVA
-- =========================
CREATE TABLE TipoviClanstva (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv VARCHAR(50) NOT NULL,
    Cena DECIMAL(10,2) NOT NULL,
    TrajanjeDana INT NOT NULL,
    MaxSatiMesecno INT NOT NULL,
    DozvoljeneSale BIT DEFAULT 0
);

-- =========================
-- KORISNICI
-- =========================
CREATE TABLE Korisnici (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Ime VARCHAR(50) NOT NULL,
    Prezime VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Telefon VARCHAR(30),
    TipClanstvaId INT,
    DatumPocetka DATE,
    DatumIsteka DATE,
    Status VARCHAR(20) DEFAULT 'aktivan',

    CONSTRAINT FK_Korisnici_TipClanstva
    FOREIGN KEY (TipClanstvaId)
    REFERENCES TipoviClanstva(Id)
);

-- =========================
-- LOKACIJE
-- =========================
CREATE TABLE Lokacije (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Naziv VARCHAR(100) NOT NULL,
    Adresa VARCHAR(200),
    Grad VARCHAR(100),
    RadnoVreme VARCHAR(100),
    MaxKorisnika INT
);

-- =========================
-- RESURSI
-- =========================
CREATE TABLE Resursi (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LokacijaId INT NOT NULL,
    Naziv VARCHAR(100),
    TipResursa VARCHAR(50),
    Opis TEXT,

    -- parametri za radna mesta
    PodTip VARCHAR(50),

    -- parametri za sale
    Kapacitet INT,
    ImaProjektor BIT DEFAULT 0,
    ImaTV BIT DEFAULT 0,
    ImaTablu BIT DEFAULT 0,
    ImaOnlineOpremu BIT DEFAULT 0,

    CONSTRAINT FK_Resursi_Lokacije
    FOREIGN KEY (LokacijaId)
    REFERENCES Lokacije(Id)
);

-- =========================
-- REZERVACIJE
-- =========================
CREATE TABLE Rezervacije (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KorisnikId INT NOT NULL,
    ResursId INT NOT NULL,
    Pocetak DATETIME NOT NULL,
    Kraj DATETIME NOT NULL,
    Status VARCHAR(20) DEFAULT 'aktivna',

    CONSTRAINT FK_Rezervacije_Korisnici
    FOREIGN KEY (KorisnikId)
    REFERENCES Korisnici(Id),

    CONSTRAINT FK_Rezervacije_Resursi
    FOREIGN KEY (ResursId)
    REFERENCES Resursi(Id)
);