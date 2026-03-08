using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingApp.BusinessLogic.Report
{
    internal class CsvExporter
    {
        //formatiranje i pravljenje .csv fajla
        public void Export(List<ReportItem> report)
        {
            string fileName = $"report_{DateTime.Now:yyyy_MM_dd_HH_mm}.csv";
            string path = Path.Combine("reports", fileName);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Ime,Prezime,BrojRezervacija,UkupnoSati,BrojResursa");

                foreach (var r in report)
                {
                    writer.WriteLine($"{r.Ime},{r.Prezime},{r.BrojRezervacija},{r.UkupnoSati},{r.BrojResursa}");
                }
            }
        }
    }
}
