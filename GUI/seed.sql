-- =========================
-- TIPOVI CLANSTVA
-- =========================
INSERT INTO TipoviClanstva (Naziv, Cena, TrajanjeDana, MaxSatiMesecno, DozvoljeneSale)
VALUES
('Dnevna karta', 500, 1, 8, 0),
('Flex desk', 5000, 30, 80, 1),
('Fiksni sto', 7000, 30, 120, 1),
('Premium', 10000, 30, 200, 1);

-- =========================
-- LOKACIJE
-- =========================
INSERT INTO Lokacije (Naziv, Adresa, Grad, RadnoVreme, MaxKorisnika)
VALUES
('Hub Kragujevac', 'Kralja Petra 12', 'Kragujevac', '08:00-20:00', 50),
('Hub Beograd', 'Nemanjina 25', 'Beograd', '07:00-22:00', 120),
('Hub Novi Sad', 'Bulevar Oslobodjenja 50', 'Novi Sad', '08:00-21:00', 80);

-- =========================
-- KORISNICI
-- =========================
INSERT INTO Korisnici (Ime, Prezime, Email, Telefon, TipClanstvaId, DatumPocetka, DatumIsteka, Status)
VALUES
('Marko', 'Markovic', 'marko@mail.com', '060111111', 2, '2026-01-01', '2026-02-01', 'aktivan'),
('Jelena', 'Jovanovic', 'jelena@mail.com', '060222222', 4, '2026-01-10', '2026-02-10', 'aktivan'),
('Petar', 'Petrovic', 'petar@mail.com', '060333333', 1, '2026-01-15', '2026-01-16', 'istekao');

-- =========================
-- RESURSI
-- =========================
INSERT INTO Resursi
(LokacijaId, Naziv, TipResursa, Opis, PodTip, Kapacitet, ImaProjektor, ImaTV, ImaTablu, ImaOnlineOpremu)
VALUES
(1, 'Desk-1', 'radno_mesto', 'Hot desk pored prozora', 'hot_desk', NULL, 0, 0, 0, 0),
(1, 'Desk-2', 'radno_mesto', 'Dedicated desk', 'dedicated_desk', NULL, 0, 0, 0, 0),
(2, 'Sala A', 'sala', 'Velika sala za sastanke', NULL, 10, 1, 1, 1, 1),
(3, 'Sala B', 'sala', 'Manja sala za sastanke', NULL, 6, 1, 1, 0, 1);

-- =========================
-- REZERVACIJE
-- =========================
INSERT INTO Rezervacije (KorisnikId, ResursId, Pocetak, Kraj, Status)
VALUES
(1, 1, '2026-03-10 09:00:00', '2026-03-10 12:00:00', 'aktivna'),
(2, 3, '2026-03-11 14:00:00', '2026-03-11 16:00:00', 'aktivna');