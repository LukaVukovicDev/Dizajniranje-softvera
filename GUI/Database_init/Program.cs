using System;
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using GUI.Services;

namespace DbSetup
{
    class Program
    {
        static IDbFactory _factory = null!;

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

            // --- 2. Fabrika odlucuje koja je baza (MSSQL ili MySQL) ---
            _factory = DbFactoryProvider.GetFactory(connectionString);
            bool isMssql = _factory is MssqlDbFactory;

            Console.WriteLine($"Brend:       {brandName}");
            Console.WriteLine($"Tip baze:    {(isMssql ? "MSSQL" : "MySQL")}");
            Console.WriteLine($"Conn string: {connectionString}\n");

            // --- 3. Putanje do skripti ---
            string dbDir = "C:\\Users\\velja\\source\\repos\\Database_init\\Database_init\\Database_init\\Databases";
            string schemaFile = isMssql
                ? Path.Combine(dbDir, "create_mssql.sql")
                : Path.Combine(dbDir, "Create_mysql.sql");
            string seedFile = Path.Combine(dbDir, "Seed.sql");

            // --- 4. Za MSSQL: schema na 'master', seed na pravoj bazi ---
            // Za MySQL: schema na 'mysql' (sistemska), seed na 'CoWorkingDB'
            string schemaConnStr = connectionString;
            string seedConnStr = connectionString;

            if (isMssql)
            {
                string dbName = ExtractDbName(connectionString);
                schemaConnStr = SwapDatabase(connectionString, "master");
                seedConnStr = SwapDatabase(connectionString, dbName);
            }
            else
            {
                // MySQL: za seed se konektujemo direktno na CoWorkingDB
                seedConnStr = SwapDatabaseMysql(connectionString, "CoWorkingDB");
            }

            // --- 5. Pokreni schema ---
            Console.WriteLine("[1/2] Kreiranje baze i tabela...");
            if (!RunSqlFile(schemaFile, schemaConnStr, isMssql)) return;
            Console.WriteLine("      OK\n");

            // --- 6. Pitaj za seed ---
            Console.Write("[2/2] Ubaciti seed podatke? (y/n): ");
            string answer = Console.ReadLine()?.Trim().ToLower();
            if (answer == "y" || answer == "yes")
            {
                if (!RunSqlFile(seedFile, seedConnStr, isMssql)) return;
                Console.WriteLine("      OK\n");

                // --- 7. Prikazi podatke u terminalu ---
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
        // PRIKAZ BAZE U TERMINALU - koristi fabriku za konekciju
        // ================================================================
        static void PrintDatabase(string connStr, bool isMssql)
        {
            Console.WriteLine();
            PrintBanner("PREGLED BAZE PODATAKA");

            // Fabrika pravi konekciju - ne znamo i ne marimo koji tip
            var factory = DbFactoryProvider.GetFactory(connStr);
            using var wrapper = factory.CreateConnection();

            // Za MSSQL eksplicitno prebaci na CoWorkingDB
            if (isMssql)
            {
                wrapper.Open();
                using var useCmd = wrapper.CreateCommand("USE CoWorkingDB");
                useCmd.ExecuteNonQuery();
            }
            else
            {
                wrapper.Open();
            }

            string fullNameExpr = isMssql
                ? "FirstName + ' ' + LastName"
                : "CONCAT(FirstName, ' ', LastName)";

            var conn = wrapper.Connection;

            PrintTableFromQuery(conn, "ADMINS",
                "SELECT Id, Username, CreatedAt FROM Admins");

            PrintTableFromQuery(conn, "MEMBERSHIP TYPES",
                "SELECT Id, Name, Price, DurationDays, MaxReservationHoursPerMonth, IncludesMeetingRooms, MeetingRoomHoursPerMonth FROM MembershipTypes");

            PrintTableFromQuery(conn, "LOCATIONS",
                "SELECT Id, Name, City, WorkingHours, MaxCapacity FROM Locations");

            PrintTableFromQuery(conn, "USERS",
                $"SELECT Id, {fullNameExpr} AS FullName, Email, MembershipTypeId, MembershipStartDate, MembershipEndDate, Status FROM Users");

            PrintTableFromQuery(conn, "RESOURCES",
                "SELECT Id, LocationId, Name, Type, IsAvailable, Capacity FROM Resources");

            PrintTableFromQuery(conn, "RESERVATIONS",
                "SELECT Id, UserId, ResourceId, StartDateTime, EndDateTime, Status FROM Reservations");
        }

        static void PrintTableFromQuery(IDbConnection conn, string title, string query)
        {
            PrintSectionHeader(title);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;

            using var reader = cmd.ExecuteReader();

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

            string separator = "+" + string.Join("+", Array.ConvertAll(colWidths, w => new string('-', w + 2))) + "+";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("|");
            for (int i = 0; i < columns.Length; i++)
                Console.Write($" {columns[i].PadRight(colWidths[i])} |");
            Console.WriteLine();
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(separator);
            Console.ResetColor();

            foreach (var row in rows)
            {
                Console.Write("|");
                for (int i = 0; i < row.Length; i++)
                {
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
                    Console.ForegroundColor = ConsoleColor.Green; break;
                case "paused":
                    Console.ForegroundColor = ConsoleColor.Yellow; break;
                case "expired":
                case "cancelled":
                case "0":
                    Console.ForegroundColor = ConsoleColor.Red; break;
                case "completed":
                    Console.ForegroundColor = ConsoleColor.DarkGray; break;
                default:
                    Console.ForegroundColor = ConsoleColor.White; break;
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
        // SQL izvrsavanje - koristi fabriku samo za schema/seed skripte
        // (moraju raw SQL jer sadrze GO i USE naredbe)
        // ================================================================
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

        // ExecuteMssql i ExecuteMysql rade direktno sa SqlConnection jer
        // moraju da hendluju GO batche i USE naredbe koje fabrika ne podrzava
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

        // Za MySQL - menja Database= vrednost u connection stringu
        static string SwapDatabaseMysql(string connStr, string newDb)
        {
            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connStr);
            builder.Database = newDb;
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