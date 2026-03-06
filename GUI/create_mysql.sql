


CREATE DATABASE IF NOT EXISTS coworking;
USE coworking;

-- =========================
-- TIPOVI CLANSTVA
-- =========================
CREATE TABLE TipoviClanstva (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    NazivPaketa VARCHAR(50) NOT NULL,
    Cena DECIMAL(10,2) NOT NULL,
    TrajanjeDani INT NOT NULL,
    MaxSatiMesecno INT NOT NULL,
    DozvoljeneSale BOOLEAN DEFAULT FALSE
);

-- =========================
-- KORISNICI
-- =========================
CREATE TABLE Korisnici (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Ime VARCHAR(50) NOT NULL,
    Prezime VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Telefon VARCHAR(30),
    TipClanstvaId INT,
    DatumPocetka DATE,
    DatumIsteka DATE,
    Status ENUM('aktivan','pauziran','istekao') DEFAULT 'aktivan',

    FOREIGN KEY (TipClanstvaId)
        REFERENCES TipoviClanstva(Id)
        ON DELETE SET NULL
);

-- =========================
-- LOKACIJE
-- =========================
CREATE TABLE Lokacije (
    Id INT AUTO_INCREMENT PRIMARY KEY,
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
    Id INT AUTO_INCREMENT PRIMARY KEY,
    LokacijaId INT NOT NULL,
    Naziv VARCHAR(100),
    TipResursa ENUM('radno_mesto','sala','privatna_kancelarija'),
    Opis TEXT,

    -- parametri za radna mesta
    PodTip ENUM('hot_desk','dedicated_desk') NULL,

    -- parametri za sale
    Kapacitet INT NULL,
    ImaProjektor BOOLEAN DEFAULT FALSE,
    ImaTV BOOLEAN DEFAULT FALSE,
    ImaTablu BOOLEAN DEFAULT FALSE,
    ImaOnlineOpremu BOOLEAN DEFAULT FALSE,

    FOREIGN KEY (LokacijaId)
        REFERENCES Lokacije(Id)
        ON DELETE CASCADE
);

-- =========================
-- REZERVACIJE
-- =========================
CREATE TABLE Rezervacije (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    KorisnikId INT NOT NULL,
    ResursId INT NOT NULL,
    Pocetak DATETIME NOT NULL,
    Kraj DATETIME NOT NULL,
    Status ENUM('aktivna','zavrsena','otkazana') DEFAULT 'aktivna',

    FOREIGN KEY (KorisnikId)
        REFERENCES Korisnici(Id)
        ON DELETE CASCADE,

    FOREIGN KEY (ResursId)
        REFERENCES Resursi(Id)
        ON DELETE CASCADE
);