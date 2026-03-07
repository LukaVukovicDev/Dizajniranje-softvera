using System.Data;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GUI.Services
{
    // ================================================================
    // ABSTRACT FACTORY PATTERN
    //
    // Struktura:
    //   IDbFactory          - apstraktna fabrika (interfejs)
    //   MssqlDbFactory      - konkretna fabrika za MSSQL
    //   MySqlDbFactory      - konkretna fabrika za MySQL
    //
    //   IDbConnectionWrapper - apstraktni produkt
    //   MssqlConnection      - konkretni produkt za MSSQL
    //   MySqlConnectionW     - konkretni produkt za MySQL
    //
    // Kako radi:
    //   DbFactoryProvider.GetFactory(connectionString) vraca
    //   odgovarajucu fabriku na osnovu connection stringa.
    //   Fabrika pravi konekciju i komande bez da ostatak koda
    //   zna da li je u pitanju MSSQL ili MySQL.
    // ================================================================


    // ----------------------------------------------------------------
    // Apstraktni produkt - konekcija
    // ----------------------------------------------------------------
    public interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection Connection { get; }
        IDbCommand CreateCommand(string sql);
        void Open();
    }

    // ----------------------------------------------------------------
    // Konkretni produkt - MSSQL konekcija
    // ----------------------------------------------------------------
    public class MssqlConnectionWrapper : IDbConnectionWrapper
    {
        public IDbConnection Connection { get; }

        public MssqlConnectionWrapper(string connStr)
        {
            Connection = new SqlConnection(connStr);
        }

        public void Open() => Connection.Open();

        public IDbCommand CreateCommand(string sql)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public void Dispose() => Connection.Dispose();
    }

    // ----------------------------------------------------------------
    // Konkretni produkt - MySQL konekcija
    // ----------------------------------------------------------------
    public class MySqlConnectionWrapper : IDbConnectionWrapper
    {
        public IDbConnection Connection { get; }

        public MySqlConnectionWrapper(string connStr)
        {
            Connection = new MySqlConnection(connStr);
        }

        public void Open() => Connection.Open();

        public IDbCommand CreateCommand(string sql)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public void Dispose() => Connection.Dispose();
    }

    // ----------------------------------------------------------------
    // Apstraktna fabrika
    // ----------------------------------------------------------------
    public interface IDbFactory
    {
        IDbConnectionWrapper CreateConnection();
        IDbDataAdapter CreateDataAdapter(string sql, IDbConnection conn);
        string ParameterPrefix { get; } // "@" za oba, ali lako prosirivo
    }

    // ----------------------------------------------------------------
    // Konkretna fabrika - MSSQL
    // ----------------------------------------------------------------
    public class MssqlDbFactory : IDbFactory
    {
        private readonly string _connStr;

        public MssqlDbFactory(string connectionString)
        {
            _connStr = connectionString;
        }

        public string ParameterPrefix => "@";

        public IDbConnectionWrapper CreateConnection()
            => new MssqlConnectionWrapper(_connStr);

        public IDbDataAdapter CreateDataAdapter(string sql, IDbConnection conn)
            => new SqlDataAdapter(sql, (SqlConnection)conn);
    }

    // ----------------------------------------------------------------
    // Konkretna fabrika - MySQL
    // ----------------------------------------------------------------
    public class MySqlDbFactory : IDbFactory
    {
        private readonly string _connStr;

        public MySqlDbFactory(string connectionString)
        {
            _connStr = connectionString;
        }

        public string ParameterPrefix => "@";

        public IDbConnectionWrapper CreateConnection()
            => new MySqlConnectionWrapper(_connStr);

        public IDbDataAdapter CreateDataAdapter(string sql, IDbConnection conn)
            => new MySqlDataAdapter(sql, (MySqlConnection)conn);
    }

    // ----------------------------------------------------------------
    // DbFactoryProvider - odlucuje koja fabrika se koristi
    // na osnovu connection stringa iz config.txt
    // ----------------------------------------------------------------
    public static class DbFactoryProvider
    {
        public static IDbFactory GetFactory(string connectionString)
        {
            return IsMssql(connectionString)
                ? new MssqlDbFactory(connectionString)
                : new MySqlDbFactory(connectionString);
        }

        private static bool IsMssql(string cs)
        {
            string lower = cs.ToLowerInvariant();
            return lower.Contains("trusted_connection") ||
                   lower.Contains("integrated security") ||
                   lower.Contains("initial catalog");
        }
    }
}