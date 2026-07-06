using SecurityBenchmark.Services;

namespace SecurityBenchmark
{
    public static class BenchmarkProgram
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine(" SECURITY PERFORMANCE BENCHMARK");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            var results = BenchmarkService.Run();
            Console.WriteLine($"Results Count = {results.Count}");

            BenchmarkService.PrintResults(results);
            CsvExportService.Export(results);
            JsonExportService.Export(results);

            Console.WriteLine();
            Console.WriteLine("CSV exported.");
            Console.WriteLine("JSON exported.");
            Console.WriteLine("Benchmark completed.");
        }
    }
}