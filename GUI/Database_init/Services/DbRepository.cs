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
                WHERE AdminID = @AdminID");
            AddParam(cmd, "@AdminID", adminId);
            AddParam(cmd, "@Username", username);
            AddParam(cmd, "@PasswordHash", passwordHash);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAdmin(int adminId)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Admins WHERE AdminID = @AdminID");
            AddParam(cmd, "@AdminID", adminId);
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
                     IncludesMeetingRooms, MeetingRoomHoursMonth, Description)
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
                    MeetingRoomHoursMonth = @MeetingRoomHours,
                    Description = @Description
                WHERE ID = @ID");
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
            using var cmd = conn.CreateCommand("DELETE FROM MembershipTypes WHERE ID = @ID");
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
                WHERE ID = @ID");
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
            using var cmd = conn.CreateCommand("DELETE FROM Locations WHERE ID = @ID");
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
                    (FirstName, LastName, Email, Phone, MembershipTypeID,
                     MembershipStartDate, MembershipEndDate, Status)
                VALUES
                    (@FirstName, @LastName, @Email, @Phone, @MembershipTypeID,
                     @MembershipStart, @MembershipEnd, @Status)");
            AddParam(cmd, "@FirstName", firstName);
            AddParam(cmd, "@LastName", lastName);
            AddParam(cmd, "@Email", email);
            AddParam(cmd, "@Phone", phone);
            AddParam(cmd, "@MembershipTypeID", membershipTypeId);
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
                    Phone = @Phone, MembershipTypeID = @MembershipTypeID,
                    MembershipStartDate = @MembershipStart,
                    MembershipEndDate = @MembershipEnd, Status = @Status
                WHERE ID = @ID");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@FirstName", firstName);
            AddParam(cmd, "@LastName", lastName);
            AddParam(cmd, "@Email", email);
            AddParam(cmd, "@Phone", phone);
            AddParam(cmd, "@MembershipTypeID", membershipTypeId);
            AddParam(cmd, "@MembershipStart", membershipStart.Date);
            AddParam(cmd, "@MembershipEnd", membershipEnd.Date);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Users WHERE ID = @ID");
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
                    (LocationID, Name, ResourceType, Description, IsAvailable,
                     Capacity, HasProjector, HasTV, HasWhiteboard, HasOnlineEquipment)
                VALUES
                    (@LocationID, @Name, @ResourceType, @Description, @IsAvailable,
                     @Capacity, @HasProjector, @HasTV, @HasWhiteboard, @HasOnlineEquipment)");
            AddParam(cmd, "@LocationID", locationId);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@ResourceType", resourceType);
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
                SET LocationID = @LocationID, Name = @Name, ResourceType = @ResourceType,
                    Description = @Description, IsAvailable = @IsAvailable,
                    Capacity = @Capacity, HasProjector = @HasProjector,
                    HasTV = @HasTV, HasWhiteboard = @HasWhiteboard,
                    HasOnlineEquipment = @HasOnlineEquipment
                WHERE ID = @ID");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@LocationID", locationId);
            AddParam(cmd, "@Name", name);
            AddParam(cmd, "@ResourceType", resourceType);
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
            using var cmd = conn.CreateCommand("DELETE FROM Resources WHERE ID = @ID");
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
                INSERT INTO Reservations (UserID, ResourceID, StartDateTime, EndDateTime, Status)
                VALUES (@UserID, @ResourceID, @StartDateTime, @EndDateTime, @Status)");
            AddParam(cmd, "@UserID", userId);
            AddParam(cmd, "@ResourceID", resourceId);
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
                SET UserID = @UserID, ResourceID = @ResourceID,
                    StartDateTime = @StartDateTime, EndDateTime = @EndDateTime,
                    Status = @Status
                WHERE ID = @ID");
            AddParam(cmd, "@ID", id);
            AddParam(cmd, "@UserID", userId);
            AddParam(cmd, "@ResourceID", resourceId);
            AddParam(cmd, "@StartDateTime", startDateTime);
            AddParam(cmd, "@EndDateTime", endDateTime);
            AddParam(cmd, "@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void CancelReservation(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand(
                "UPDATE Reservations SET Status = 'cancelled' WHERE ID = @ID");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteReservation(int id)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand("DELETE FROM Reservations WHERE ID = @ID");
            AddParam(cmd, "@ID", id);
            cmd.ExecuteNonQuery();
        }
    }
}