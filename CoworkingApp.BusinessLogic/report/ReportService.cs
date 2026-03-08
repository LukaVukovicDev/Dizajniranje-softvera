using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingApp.BusinessLogic.Report
{
    internal class ReportService
    {
        private string connectionString;

        public ReportService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<ReportItem> GetMonthlyReport()
        {
            var report = new Dictionary<string, ReportItem>();

            /*racuna sate van upita kako bi radilo univerzalno za svaki jezik
              pa zato sate racuna nakon upita*/
            string query = @"
        SELECT 
            k.Ime,
            k.Prezime,
            r.DatumPocetka,
            r.DatumZavrsetka,
            r.IdResursa
        FROM Rezervacije r
        JOIN Korisnici k ON r.IdKorisnika = k.IdKorisnika
        WHERE 
            MONTH(r.DatumPocetka) = MONTH(CURRENT_TIMESTAMP)
            AND YEAR(r.DatumPocetka) = YEAR(CURRENT_TIMESTAMP)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ime = reader.GetString(0);
                        string prezime = reader.GetString(1);
                        DateTime start = reader.GetDateTime(2);
                        DateTime end = reader.GetDateTime(3);
                        int resourceId = reader.GetInt32(4);

                        string key = ime + "_" + prezime;

                        if (!report.ContainsKey(key))
                        {
                            report[key] = new ReportItem
                            {
                                Ime = ime,
                                Prezime = prezime,
                                BrojRezervacija = 0,
                                UkupnoSati = 0,
                                BrojResursa = 0
                            };
                        }

                        var item = report[key];

                        item.BrojRezervacija++;

                        TimeSpan diff = end - start;
                        item.UkupnoSati += (int)diff.TotalHours;

                        item.BrojResursa++;
                    }
                }
            }

            return new List<ReportItem>(report.Values);
        }
    }
}
