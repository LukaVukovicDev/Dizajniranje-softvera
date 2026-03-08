using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CoworkingApp.BusinessLogic.Report
{
    internal class ReportScheduler
    {
        private Timer timer;
        private ReportService reportService;
        private CsvExporter exporter;

        public ReportScheduler(string connectionString, double interval)
        {
            reportService = new ReportService(connectionString);
            exporter = new CsvExporter();

            timer = new Timer(interval);
            timer.Elapsed += GenerateReport;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
        }

        private void GenerateReport(object sender, ElapsedEventArgs e)
        {
            var report = reportService.GetMonthlyReport();
            exporter.Export(report);
        }
    }
}
