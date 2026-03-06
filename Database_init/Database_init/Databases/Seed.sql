-- ============================================================
-- Seed.sql - Inicijalni podaci za Co-working sistem
-- Imena kolona uskladjena sa MySQL schemom
-- ============================================================
USE CoWorkingDB;
GO

-- Admins (lozinka: "admin123" - BCrypt hash, zameniti pravim hashom)
INSERT INTO Admins (Username, PasswordHash) VALUES
('admin', '$2a$11$KmvL1QJ7J9Q1Q1Q1Q1Q1QeQ1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1Q1');
GO

-- MembershipTypes
INSERT INTO MembershipTypes (Name, Price, DurationDays, MaxReservationHoursPerMonth, IncludesMeetingRooms, MeetingRoomHoursMonth, Description) VALUES
('Dnevna karta',     500.00,  1,   8,  0,  0, 'Pristup prostoru na jedan dan, bez rezervacije sala.'),
('Fleksibilni sto',  8000.00, 30,  40, 0,  0, 'Hot desk po izboru, 40h/mes, bez sala za sastanke.'),
('Fiksni sto',      12000.00, 30,  80, 1,  4, 'Dedicated desk, 80h/mes, ukljuceno 4h sala mesecno.'),
('Premium',         18000.00, 30, 160, 1, 10, 'Neogranicen pristup, 10h sala mesecno, privatna kancelarija.');
GO

-- Locations
INSERT INTO Locations (Name, Address, City, WorkingHours, MaxCapacity, Description) VALUES
('HubSpace Kragujevac', 'Ulica Kneza Milosa 14',  'Kragujevac', '08:00 - 22:00', 60,  'Glavni hub u centru Kragujevca.'),
('HubSpace Beograd',    'Bulevar Oslobodjenja 5', 'Beograd',    '07:00 - 23:00', 120, 'Najveci hub, savremeno opremljen.'),
('HubSpace Novi Sad',   'Zmaj Jovina 22',         'Novi Sad',   '08:00 - 21:00', 45,  'Hub u srcu Novog Sada.');
GO

-- Resources - Kragujevac (LocationID = 1)
INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable) VALUES
(1, 'HD-01', 'hot_desk',       'Hot desk, prozorska strana',      1),
(1, 'HD-02', 'hot_desk',       'Hot desk, centralni deo',         1),
(1, 'HD-03', 'hot_desk',       'Hot desk, mirna zona',            1),
(1, 'DD-01', 'dedicated_desk', 'Fiksni sto, ergonomska stolica',  1),
(1, 'DD-02', 'dedicated_desk', 'Fiksni sto, dual monitor setup',  0),
(1, 'PO-01', 'private_office', 'Privatna kancelarija za 2 osobe', 1);
GO

INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable, Capacity, HasProjector, HasTV, HasWhiteboard, HasOnlineEquipment) VALUES
(1, 'Sala Morava',   'meeting_room', 'Sala za manje sastanke',    1,  6, 0, 1, 1, 1),
(1, 'Sala Sumadija', 'meeting_room', 'Velika konferencijska sala', 1, 20, 1, 1, 1, 1);
GO

-- Resources - Beograd (LocationID = 2)
INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable) VALUES
(2, 'HD-01', 'hot_desk',       'Hot desk, open space',    1),
(2, 'HD-02', 'hot_desk',       'Hot desk, quiet zone',    1),
(2, 'HD-03', 'hot_desk',       'Hot desk, standing desk', 1),
(2, 'DD-01', 'dedicated_desk', 'Fiksni sto A zona',       1),
(2, 'DD-02', 'dedicated_desk', 'Fiksni sto B zona',       1),
(2, 'PO-01', 'private_office', 'Kancelarija za 4 osobe',  1),
(2, 'PO-02', 'private_office', 'Kancelarija za 6 osoba',  0);
GO

INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable, Capacity, HasProjector, HasTV, HasWhiteboard, HasOnlineEquipment) VALUES
(2, 'Sala Tesla',  'meeting_room', 'Premium sala sa punom opremom', 1, 12, 1, 1, 1, 1),
(2, 'Sala Nikola', 'meeting_room', 'Manja sala za timove',          1,  6, 0, 1, 1, 0),
(2, 'Sala Vuk',    'meeting_room', 'Velika konferencijska sala',    1, 30, 1, 1, 1, 1);
GO

-- Resources - Novi Sad (LocationID = 3)
INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable) VALUES
(3, 'HD-01', 'hot_desk',       'Hot desk, prizemlje',    1),
(3, 'HD-02', 'hot_desk',       'Hot desk, prvi sprat',   1),
(3, 'DD-01', 'dedicated_desk', 'Fiksni sto, prozor',     1),
(3, 'PO-01', 'private_office', 'Kancelarija za 3 osobe', 1);
GO

INSERT INTO Resources (LocationID, Name, ResourceType, Description, IsAvailable, Capacity, HasProjector, HasTV, HasWhiteboard, HasOnlineEquipment) VALUES
(3, 'Sala Danube', 'meeting_room', 'Sala sa pogledom na park', 1, 8, 1, 0, 1, 1);
GO

-- Users (MembershipTypeID: 1=Dnevna, 2=Fleksibilni, 3=Fiksni, 4=Premium)
INSERT INTO Users (FirstName, LastName, Email, Phone, MembershipTypeID, MembershipStartDate, MembershipEndDate, Status) VALUES
('Marko',   'Petrovic',   'marko.petrovic@email.com',   '0641234567', 4, '2025-01-01', '2025-12-31', 'active'),
('Jelena',  'Nikolic',    'jelena.nikolic@email.com',   '0652345678', 3, '2025-02-01', '2026-01-31', 'active'),
('Stefan',  'Jovanovic',  'stefan.jovanovic@email.com', '0663456789', 2, '2025-03-01', '2026-02-28', 'active'),
('Ana',     'Stojanovic', 'ana.stojanovic@email.com',   '0674567890', 2, '2025-01-15', '2026-01-14', 'active'),
('Nikola',  'Ilic',       'nikola.ilic@email.com',      '0685678901', 1, '2025-03-06', '2025-03-06', 'active'),
('Milica',  'Popovic',    'milica.popovic@email.com',   '0696789012', 3, '2024-10-01', '2025-09-30', 'active'),
('Aleksa',  'Djordjevic', 'aleksa.djordjevic@email.com','0607890123', 4, '2025-01-01', '2025-12-31', 'active'),
('Tamara',  'Markovic',   'tamara.markovic@email.com',  '0618901234', 2, '2024-12-01', '2025-11-30', 'paused'),
('Dragana', 'Lukic',      'dragana.lukic@email.com',    '0629012345', 1, '2025-02-01', '2025-02-01', 'expired'),
('Ivan',    'Mijailovic', 'ivan.mijailovic@email.com',  '0630123456', 3, '2025-03-01', '2026-02-28', 'active');
GO

-- Reservations
INSERT INTO Reservations (UserID, ResourceID, StartDateTime, EndDateTime, Status) VALUES
(1,  8, '2025-03-06 09:00:00', '2025-03-06 11:00:00', 'active'),
(2,  4, '2025-03-06 08:00:00', '2025-03-16 18:00:00', 'active'),
(3,  1, '2025-03-07 10:00:00', '2025-03-07 14:00:00', 'active'),
(4,  9, '2025-03-07 13:00:00', '2025-03-07 15:00:00', 'active'),
(6, 15, '2025-03-05 09:00:00', '2025-03-05 12:00:00', 'completed'),
(7, 16, '2025-03-04 14:00:00', '2025-03-04 16:00:00', 'completed'),
(1,  8, '2025-02-20 10:00:00', '2025-02-20 12:00:00', 'completed'),
(3,  2, '2025-03-10 09:00:00', '2025-03-10 13:00:00', 'active'),
(10, 5, '2025-03-06 11:00:00', '2025-03-06 15:00:00', 'active'),
(2,  8, '2025-03-08 09:00:00', '2025-03-08 11:00:00', 'cancelled');
GO