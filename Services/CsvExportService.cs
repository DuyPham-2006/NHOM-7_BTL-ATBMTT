using CsvHelper;
using CsvHelper.Configuration;
using SecurityBenchmark.Models;
using System.Globalization;

namespace SecurityBenchmark.Services
{
    public static class CsvExportService
    {
        public static void Export(List<BenchmarkResult> results)
        {
            Directory.CreateDirectory("Output");

            using var writer = new StreamWriter("Output/benchmark.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using var csv = new CsvWriter(writer, config);

            csv.WriteRecords(results);
        }
    }
}