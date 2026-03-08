using System;
using System.Data;

namespace GUI.Services
{
    // ================================================================
    // DbRepository - koristi IDbFactory (Abstract Factory pattern)
    // umesto direktnog new SqlConnection / new MySqlConnection
    //
    // Primer koriscenja:
    //   var factory = DbFactoryProvider.GetFactory(connectionString);
    //   var repo    = new DbRepository(factory);
    //   repo.InsertUser(...);
    // ================================================================
    public class DbRepository
    {
        private readonly IDbFactory _factory;

        public DbRepository(IDbFactory factory)
        {
            _factory = factory;
        }

        // ----------------------------------------------------------------
        // Pomocna metoda - kreira konekciju kroz fabriku
        // ----------------------------------------------------------------
        private IDbConnectionWrapper OpenConnection()
        {
            var conn = _factory.CreateConnection();
            conn.Open();
            return conn;
        }

        private static void AddParam(IDbCommand cmd, string name, object? value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        // ================================================================
        // ADMINS
        // ================================================================
        public void InsertAdmin(string username, string passwordHash)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO Admins (Username, PasswordHash)
                VALUES (@Username, @PasswordHash)");
            AddParam(cmd, "@Username", username);
            AddParam(cmd, "@PasswordHash", passwordHash);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAdmin(int adminId, string username, string passwordHash)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE Admins
                SET Username = @Username, PasswordHash = @PasswordHash
                WHERE Id = @Id");
            AddParam(cmd, "@Id", adminId);
            AddParam(cmd, "@Username", username);
            AddParam(cmd, "@PasswordHash", passwordHash);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAdmin(int adminId)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Admins WHERE Id = @Id");
            AddParam(cmd, "@Id", adminId);
            cmd.ExecuteNonQuery();
        }

        // ================================================================
        // MEMBERSHIP TYPES
        // ================================================================
        public void InsertMembershipType(string name, decimal price, int durationDays,
            int maxHoursPerMonth, bool includesMeetingRooms, int meetingRoomHours, string description)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO MembershipTypes
                    (Name, Price, DurationDays, MaxReservationHoursPerMonth,
                     IncludesMeetingRooms, MeetingRoomHoursPerMonth, Description)
                VALUES
                    (@Name, @Price, @DurationDays, @MaxHours,
                     @IncludesMeetingRooms, @MeetingRoomHours, @Description)");
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Price", price);
            AddParam(cmd, "@DurationDays", durationDays);
            AddParam(cmd, "@MaxHours", maxHoursPerMonth);
            AddParam(cmd, "@IncludesMeetingRooms", includesMeetingRooms ? 1 : 0);
            AddParam(cmd, "@MeetingRoomHours", meetingRoomHours);
            AddParam(cmd, "@Description", description);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMembershipType(int id, string name, decimal price, int durationDays,
            int maxHoursPerMonth, bool includesMeetingRooms, int meetingRoomHours, string description)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE MembershipTypes
                SET Name = @Name, Price = @Price, DurationDays = @DurationDays,
                    MaxReservationHoursPerMonth = @MaxHours,
                    IncludesMeetingRooms = @IncludesMeetingRooms,
                    MeetingRoomHoursPerMonth = @MeetingRoomHours,
                    Description = @Description
                WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Price", price);
            AddParam(cmd, "@DurationDays", durationDays);
            AddParam(cmd, "@MaxHours", maxHoursPerMonth);
            AddParam(cmd, "@IncludesMeetingRooms", includesMeetingRooms ? 1 : 0);
            AddParam(cmd, "@MeetingRoomHours", meetingRoomHours);
            AddParam(cmd, "@Description", description);
            cmd.ExecuteNonQuery();
        }

        public void DeleteMembershipType(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM MembershipTypes WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        // ================================================================
        // LOCATIONS
        // ================================================================
        public void InsertLocation(string name, string address, string city,
            string workingHours, int maxCapacity, string description)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO Locations (Name, Address, City, WorkingHours, MaxCapacity, Description)
                VALUES (@Name, @Address, @City, @WorkingHours, @MaxCapacity, @Description)");
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Address", address);
            AddParam(cmd, "@City", city);
            AddParam(cmd, "@WorkingHours", workingHours);
            AddParam(cmd, "@MaxCapacity", maxCapacity);
            AddParam(cmd, "@Description", description);
            cmd.ExecuteNonQuery();
        }

        public void UpdateLocation(int id, string name, string address, string city,
            string workingHours, int maxCapacity, string description)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE Locations
                SET Name = @Name, Address = @Address, City = @City,
                    WorkingHours = @WorkingHours, MaxCapacity = @MaxCapacity,
                    Description = @Description
                WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Address", address);
            AddParam(cmd, "@City", city);
            AddParam(cmd, "@WorkingHours", workingHours);
            AddParam(cmd, "@MaxCapacity", maxCapacity);
            AddParam(cmd, "@Description", description);
            cmd.ExecuteNonQuery();
        }

        public void DeleteLocation(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Locations WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        // ================================================================
        // USERS
        // ================================================================
        public void InsertUser(string firstName, string lastName, string email,
            string phone, int membershipTypeId, DateTime membershipStart,
            DateTime membershipEnd, string status)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO Users
                    (FirstName, LastName, Email, Phone, MembershipTypeId,
                     MembershipStartDate, MembershipEndDate, Status)
                VALUES
                    (@FirstName, @LastName, @Email, @Phone, @MembershipTypeId,
                     @MembershipStart, @MembershipEnd, @Status)");
            AddParam(cmd, "@FirstName", firstName);
            AddParam(cmd, "@LastName", lastName);
            AddParam(cmd, "@Email", email);
            AddParam(cmd, "@Phone", phone);
            AddParam(cmd, "@MembershipTypeId", membershipTypeId);
            AddParam(cmd, "@MembershipStart", membershipStart.Date);
            AddParam(cmd, "@MembershipEnd", membershipEnd.Date);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void UpdateUser(int id, string firstName, string lastName, string email,
            string phone, int membershipTypeId, DateTime membershipStart,
            DateTime membershipEnd, string status)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE Users
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email,
                    Phone = @Phone, MembershipTypeId = @MembershipTypeId,
                    MembershipStartDate = @MembershipStart,
                    MembershipEndDate = @MembershipEnd, Status = @Status
                WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@FirstName", firstName);
            AddParam(cmd, "@LastName", lastName);
            AddParam(cmd, "@Email", email);
            AddParam(cmd, "@Phone", phone);
            AddParam(cmd, "@MembershipTypeId", membershipTypeId);
            AddParam(cmd, "@MembershipStart", membershipStart.Date);
            AddParam(cmd, "@MembershipEnd", membershipEnd.Date);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Users WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        // ================================================================
        // RESOURCES
        // ================================================================
        public void InsertResource(int locationId, string name, string resourceType,
            string description, bool isAvailable, int? capacity = null,
            bool hasProjector = false, bool hasTV = false,
            bool hasWhiteboard = false, bool hasOnlineEquipment = false)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO Resources
                    (LocationId, Name, Type, Description, IsAvailable,
                     Capacity, HasProjector, HasTV, HasWhiteboard, HasOnlineEquipment)
                VALUES
                    (@LocationId, @Name, @Type, @Description, @IsAvailable,
                     @Capacity, @HasProjector, @HasTV, @HasWhiteboard, @HasOnlineEquipment)");
            AddParam(cmd, "@LocationId", locationId);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Type", resourceType);
            AddParam(cmd, "@Description", description);
            AddParam(cmd, "@IsAvailable", isAvailable ? 1 : 0);
            AddParam(cmd, "@Capacity", (object?)capacity ?? DBNull.Value);
            AddParam(cmd, "@HasProjector", hasProjector ? 1 : 0);
            AddParam(cmd, "@HasTV", hasTV ? 1 : 0);
            AddParam(cmd, "@HasWhiteboard", hasWhiteboard ? 1 : 0);
            AddParam(cmd, "@HasOnlineEquipment", hasOnlineEquipment ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void UpdateResource(int id, int locationId, string name, string resourceType,
            string description, bool isAvailable, int? capacity = null,
            bool hasProjector = false, bool hasTV = false,
            bool hasWhiteboard = false, bool hasOnlineEquipment = false)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE Resources
                SET LocationId = @LocationId, Name = @Name, Type = @Type,
                    Description = @Description, IsAvailable = @IsAvailable,
                    Capacity = @Capacity, HasProjector = @HasProjector,
                    HasTV = @HasTV, HasWhiteboard = @HasWhiteboard,
                    HasOnlineEquipment = @HasOnlineEquipment
                WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@LocationId", locationId);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@Type", resourceType);
            AddParam(cmd, "@Description", description);
            AddParam(cmd, "@IsAvailable", isAvailable ? 1 : 0);
            AddParam(cmd, "@Capacity", (object?)capacity ?? DBNull.Value);
            AddParam(cmd, "@HasProjector", hasProjector ? 1 : 0);
            AddParam(cmd, "@HasTV", hasTV ? 1 : 0);
            AddParam(cmd, "@HasWhiteboard", hasWhiteboard ? 1 : 0);
            AddParam(cmd, "@HasOnlineEquipment", hasOnlineEquipment ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void DeleteResource(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Resources WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        // ================================================================
        // RESERVATIONS
        // ================================================================
        public void InsertReservation(int userId, int resourceId,
            DateTime startDateTime, DateTime endDateTime, string status = "active")
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                INSERT INTO Reservations (UserId, ResourceId, StartDateTime, EndDateTime, Status)
                VALUES (@UserId, @ResourceId, @StartDateTime, @EndDateTime, @Status)");
            AddParam(cmd, "@UserId", userId);
            AddParam(cmd, "@ResourceId", resourceId);
            AddParam(cmd, "@StartDateTime", startDateTime);
            AddParam(cmd, "@EndDateTime", endDateTime);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void UpdateReservation(int id, int userId, int resourceId,
            DateTime startDateTime, DateTime endDateTime, string status)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(@"
                UPDATE Reservations
                SET UserId = @UserId, ResourceId = @ResourceId,
                    StartDateTime = @StartDateTime, EndDateTime = @EndDateTime,
                    Status = @Status
                WHERE Id = @Id");
            AddParam(cmd, "@Id", id);
            AddParam(cmd, "@UserId", userId);
            AddParam(cmd, "@ResourceId", resourceId);
            AddParam(cmd, "@StartDateTime", startDateTime);
            AddParam(cmd, "@EndDateTime", endDateTime);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void CancelReservation(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(
                "UPDATE Reservations SET Status = 'cancelled' WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteReservation(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Reservations WHERE Id = @Id");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }
    }
}