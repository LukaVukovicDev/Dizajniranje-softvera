using System;
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace DbSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Co-working DB Setup ===\n");

            // --- 1. Ucitaj config.txt ---
            string configPath = "C:\\Users\\velja\\source\\repos\\Database_init\\Database_init\\Database_init\\config.txt";
            if (!File.Exists(configPath))
            {
                Error("Nije pronadjen config.txt.");
                return;
            }

            string[] lines = File.ReadAllLines(configPath);
            if (lines.Length < 2)
            {
                Error("config.txt mora imati 2 linije: naziv brenda i connection string.");
                return;
            }

            string brandName = lines[0].Trim();
            string connectionString = lines[1].Trim();
            bool isMssql = DetectMssql(connectionString);

            Console.WriteLine($"Brend:       {brandName}");
            Console.WriteLine($"Tip baze:    {(isMssql ? "MSSQL" : "MySQL")}");
            Console.WriteLine($"Conn string: {connectionString}\n");

            // --- 2. Putanje do skripti ---
            string dbDir = "C:\\Users\\velja\\source\\repos\\Database_init\\Database_init\\Database_init\\Databases";
            string schemaFile = isMssql
                ? Path.Combine(dbDir, "create_mssql.sql")
                : Path.Combine(dbDir, "Create_mysql.sql");
            string seedFile = Path.Combine(dbDir, "Seed.sql");

            // --- 3. Za MSSQL: schema na 'master', seed na pravoj bazi ---
            string schemaConnStr = connectionString;
            string seedConnStr = connectionString;

            if (isMssql)
            {
                string dbName = ExtractDbName(connectionString);
                schemaConnStr = SwapDatabase(connectionString, "master");
                seedConnStr = SwapDatabase(connectionString, dbName);
            }

            // --- 4. Pokreni schema ---
            Console.WriteLine("[1/2] Kreiranje baze i tabela...");
            if (!RunSqlFile(schemaFile, schemaConnStr, isMssql)) return;
            Console.WriteLine("      OK\n");

            // --- 5. Pitaj za seed ---
            Console.Write("[2/2] Ubaciti seed podatke? (y/n): ");
            string answer = Console.ReadLine()?.Trim().ToLower();
            if (answer == "y" || answer == "yes")
            {
                if (!RunSqlFile(seedFile, seedConnStr, isMssql)) return;
                Console.WriteLine("      OK\n");

                // --- 6. Prikazi podatke u terminalu ---
                Console.Write("Prikazati podatke iz baze? (y/n): ");
                string showData = Console.ReadLine()?.Trim().ToLower();
                if (showData == "y" || showData == "yes")
                    PrintDatabase(seedConnStr, isMssql);
            }
            else
            {
                Console.WriteLine("      Preskoceno.\n");
            }

            Console.WriteLine("\n=== Gotovo! Baza je spremna. ===");
            Console.ReadKey();
        }

        // ================================================================
        // PRIKAZ BAZE U TERMINALU
        // ================================================================
        static void PrintDatabase(string connStr, bool isMssql)
        {
            Console.WriteLine();
            PrintBanner("PREGLED BAZE PODATAKA");

            using IDbConnection conn = isMssql
                ? new SqlConnection(connStr)
                : new MySqlConnection(connStr);
            conn.Open();

            // Za MSSQL eksplicitno se prebaci na CoWorkingDB
            if (isMssql)
            {
                using var useCmd = ((SqlConnection)conn).CreateCommand();
                useCmd.CommandText = "USE CoWorkingDB";
                useCmd.ExecuteNonQuery();
            }

            // MSSQL koristi + za spajanje stringa, MySQL koristi CONCAT()
            string fullNameExpr = isMssql
                ? "FirstName + ' ' + LastName"
                : "CONCAT(FirstName, ' ', LastName)";

            PrintTableFromQuery(conn, "ADMINS",
                "SELECT AdminID, Username, CreatedAt FROM Admins");

            PrintTableFromQuery(conn, "MEMBERSHIP TYPES",
                "SELECT ID, Name, Price, DurationDays, MaxReservationHoursPerMonth, IncludesMeetingRooms, MeetingRoomHoursMonth FROM MembershipTypes");

            PrintTableFromQuery(conn, "LOCATIONS",
                "SELECT ID, Name, City, WorkingHours, MaxCapacity FROM Locations");

            PrintTableFromQuery(conn, "USERS",
                $"SELECT ID, {fullNameExpr} AS FullName, Email, MembershipTypeID, MembershipStartDate, MembershipEndDate, Status FROM Users");

            PrintTableFromQuery(conn, "RESOURCES",
                "SELECT ID, LocationID, Name, ResourceType, IsAvailable, Capacity FROM Resources");

            PrintTableFromQuery(conn, "RESERVATIONS",
                "SELECT ID, UserID, ResourceID, StartDateTime, EndDateTime, Status FROM Reservations");
        }

        static void PrintTableFromQuery(IDbConnection conn, string title, string query)
        {
            PrintSectionHeader(title);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();

            // Ucitaj sve redove i kolone
            var columns = new string[reader.FieldCount];
            var rows = new System.Collections.Generic.List<string[]>();
            var colWidths = new int[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = reader.GetName(i);
                colWidths[i] = columns[i].Length;
            }

            while (reader.Read())
            {
                var row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString() ?? "";
                    colWidths[i] = Math.Max(colWidths[i], row[i].Length);
                }
                rows.Add(row);
            }

            // Iscrtaj tabelu
            string separator = "+" + string.Join("+", Array.ConvertAll(colWidths, w => new string('-', w + 2))) + "+";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();

            // Header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("|");
            for (int i = 0; i < columns.Length; i++)
                Console.Write($" {columns[i].PadRight(colWidths[i])} |");
            Console.WriteLine();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();

            // Redovi
            foreach (var row in rows)
            {
                Console.Write("|");
                for (int i = 0; i < row.Length; i++)
                {
                    // Obojanaj status kolonu
                    if (columns[i].ToLower() == "status" || columns[i].ToLower() == "isavailable")
                    {
                        Console.Write(" ");
                        ColorizeValue(row[i], colWidths[i]);
                        Console.Write(" |");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" {row[i].PadRight(colWidths[i])} |");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  Ukupno redova: {rows.Count}");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void ColorizeValue(string value, int width)
        {
            switch (value.ToLower())
            {
                case "active":
                case "1":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "paused":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "expired":
                case "cancelled":
                case "0":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "completed":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(value.PadRight(width));
            Console.ResetColor();
        }

        static void PrintBanner(string text)
        {
            string line = new string('=', text.Length + 4);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(line);
            Console.WriteLine($"  {text}");
            Console.WriteLine(line);
            Console.ResetColor();
            Console.WriteLine();
        }

        static void PrintSectionHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"  >> {text}");
            Console.ResetColor();
        }

        // ================================================================
        // MSSQL / MySQL izvrsavanje
        // ================================================================
        static bool DetectMssql(string cs)
        {
            string lower = cs.ToLowerInvariant();
            if (lower.Contains("trusted_connection") ||
                lower.Contains("integrated security") ||
                lower.Contains("initial catalog"))
                return true;

            if (lower.Contains("uid=") ||
                lower.Contains("sslmode") ||
                lower.Contains("port=3306") ||
                lower.Contains("allowuservariables"))
                return false;

            return false;
        }

        static bool RunSqlFile(string filePath, string connStr, bool isMssql)
        {
            if (!File.Exists(filePath))
            {
                Error($"Nije pronadjen fajl: {filePath}");
                return false;
            }

            string sql = File.ReadAllText(filePath);

            try
            {
                if (isMssql) ExecuteMssql(sql, connStr);
                else ExecuteMysql(sql, connStr);
                return true;
            }
            catch (Exception ex)
            {
                Error($"Greska pri izvrsavanju {Path.GetFileName(filePath)}:\n{ex.Message}");
                return false;
            }
        }

        static void ExecuteMssql(string sql, string connStr)
        {
            using var conn = new SqlConnection(connStr);
            conn.Open();

            string normalized = sql.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] batches = System.Text.RegularExpressions.Regex.Split(
                normalized,
                @"^\s*GO\s*$",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            foreach (string batch in batches)
            {
                string trimmed = batch.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                var useMatch = System.Text.RegularExpressions.Regex.Match(
                    trimmed,
                    @"^USE\s+\[?(\w+)\]?\s*;?$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (useMatch.Success)
                {
                    conn.ChangeDatabase(useMatch.Groups[1].Value);
                    Console.WriteLine($"      >> Prebaceno na bazu: {useMatch.Groups[1].Value}");
                    continue;
                }

                using var cmd = new SqlCommand(trimmed, conn);
                cmd.CommandTimeout = 60;
                cmd.ExecuteNonQuery();
            }
        }

        static void ExecuteMysql(string sql, string connStr)
        {
            using var conn = new MySqlConnection(connStr);
            conn.Open();
            new MySqlScript(conn, sql).Execute();
        }

        static string ExtractDbName(string connStr)
        {
            var builder = new SqlConnectionStringBuilder(connStr);
            return string.IsNullOrWhiteSpace(builder.InitialCatalog) ? "CoWorkingDB" : builder.InitialCatalog;
        }

        static string SwapDatabase(string connStr, string newDb)
        {
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.InitialCatalog = newDb;
            return builder.ConnectionString;
        }

        static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[GRESKA] {msg}");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}