using System.Diagnostics;
using SecurityBenchmark.Models;

namespace SecurityBenchmark.Services
{
    public static class BenchmarkService
    {
        private const int Iterations = 100;
        private const string Password = "Password@123";

        public static List<BenchmarkResult> Run()
        {
            var results = new List<BenchmarkResult>();

            Console.WriteLine("========================================");
            Console.WriteLine(" SECURITY PERFORMANCE BENCHMARK");
            Console.WriteLine("========================================");
            Console.WriteLine();

            Console.WriteLine("Running SHA256...");
            results.Add(BenchmarkSHA256());

            Console.WriteLine("Running BCrypt Cost = 8...");
            results.Add(BenchmarkBCrypt(8));

            Console.WriteLine("Running BCrypt Cost = 10...");
            results.Add(BenchmarkBCrypt(10));

            Console.WriteLine("Running BCrypt Cost = 12...");
            results.Add(BenchmarkBCrypt(12));

            Console.WriteLine("Running BCrypt Cost = 14...");
            results.Add(BenchmarkBCrypt(14));

            Console.WriteLine("Running Argon2id (32 MB)...");
            results.Add(BenchmarkArgon2(32768));

            Console.WriteLine("Running Argon2id (64 MB)...");
            results.Add(BenchmarkArgon2(65536));

            Console.WriteLine("Running Argon2id (128 MB)...");
            results.Add(BenchmarkArgon2(131072));

            return results;
        }

        private static BenchmarkResult BenchmarkSHA256()
        {
            List<double> hashTimes = new();
            List<double> verifyTimes = new();

            for (int i = 0; i < Iterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                string hash = HashService.HashSHA256(Password);

                sw.Stop();
                hashTimes.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();

                bool ok = HashService.HashSHA256(Password) == hash;

                sw.Stop();
                verifyTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            return BuildResult(
                "SHA-256",
                "Default",
                hashTimes,
                verifyTimes,
                "Không dùng Salt");
        }

        private static BenchmarkResult BenchmarkBCrypt(int cost)
        {
            List<double> hashTimes = new();
            List<double> verifyTimes = new();

            for (int i = 0; i < Iterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                string hash = HashService.HashBCrypt(Password, cost);

                sw.Stop();
                hashTimes.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();

                HashService.VerifyBCrypt(Password, hash);

                sw.Stop();
                verifyTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            return BuildResult(
                "BCrypt",
                $"Cost={cost}",
                hashTimes,
                verifyTimes,
                $"WorkFactor={cost}");
        }

        private static BenchmarkResult BenchmarkArgon2(int memory)
        {
            List<double> hashTimes = new();
            List<double> verifyTimes = new();

            for (int i = 0; i < Iterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                string hash = HashService.HashArgon2id(
                    Password,
                    iterations: 4,
                    memorySize: memory,
                    parallelism: 2).GetAwaiter().GetResult();

                sw.Stop();

                hashTimes.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();

                HashService.VerifyArgon2id(
                    Password,
                    hash).GetAwaiter().GetResult();

                sw.Stop();

                verifyTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            return BuildResult(
                "Argon2id",
                $"{memory / 1024} MB",
                hashTimes,
                verifyTimes,
                $"Memory={memory / 1024}MB");
        }
                private static BenchmarkResult BuildResult(
            string algorithm,
            string configuration,
            List<double> hashTimes,
            List<double> verifyTimes,
            string note)
        {
            return new BenchmarkResult
            {
                Algorithm = algorithm,
                Configuration = configuration,
                Iterations = Iterations,

                AverageHashTime = hashTimes.Average(),
                AverageVerifyTime = verifyTimes.Average(),

                MinHashTime = hashTimes.Min(),
                MaxHashTime = hashTimes.Max(),

                MinVerifyTime = verifyTimes.Min(),
                MaxVerifyTime = verifyTimes.Max(),

                Note = note
            };
        }

        public static void PrintResults(List<BenchmarkResult> results)
        {
            Console.WriteLine();
            Console.WriteLine("==============================================================================================");
            Console.WriteLine(
                "{0,-12} {1,-12} {2,12} {3,12} {4,12} {5,12}",
                "Algorithm",
                "Config",
                "Avg Hash",
                "Avg Verify",
                "Min",
                "Max");

            Console.WriteLine("----------------------------------------------------------------------------------------------");

            foreach (var r in results)
            {
                Console.WriteLine(
                    "{0,-12} {1,-12} {2,12:F2} {3,12:F2} {4,12:F2} {5,12:F2}",
                    r.Algorithm,
                    r.Configuration,
                    r.AverageHashTime,
                    r.AverageVerifyTime,
                    r.MinHashTime,
                    r.MaxHashTime);
            }

            Console.WriteLine("==============================================================================================");
        }
    }
}