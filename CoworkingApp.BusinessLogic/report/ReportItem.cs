using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingApp.BusinessLogic.Report
{
    internal class ReportItem
    {
        //forma .csv fajla koji ce biti exportovan
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int BrojRezervacija { get; set; }
        public int UkupnoSati { get; set; }
        public int BrojResursa { get; set; }
    }
}
