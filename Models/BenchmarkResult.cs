namespace SecurityBenchmark.Models
{
    public class BenchmarkResult
    {
        public string Algorithm { get; set; } = "";

        public string Configuration { get; set; } = "";

        public int Iterations { get; set; }

        public double AverageHashTime { get; set; }

        public double AverageVerifyTime { get; set; }

        public double MinHashTime { get; set; }

        public double MaxHashTime { get; set; }

        public double MinVerifyTime { get; set; }

        public double MaxVerifyTime { get; set; }

        public string Note { get; set; } = "";
    }
}