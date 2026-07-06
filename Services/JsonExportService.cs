using System.Text.Json;
using SecurityBenchmark.Models;

namespace SecurityBenchmark.Services
{
    public static class JsonExportService
    {
        public static void Export(List<BenchmarkResult> results)
        {
            Directory.CreateDirectory("Output");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(results, options);

            File.WriteAllText("Output/benchmark.json", json);
        }
    }
}