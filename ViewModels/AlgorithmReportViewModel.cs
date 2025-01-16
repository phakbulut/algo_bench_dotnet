namespace algo.ViewModels
{
    public class AlgorithmReportViewModel
    {
        public string Algorithm { get; set; }
        public string ArrayType { get; set; }
        public string ArraySize { get; set; }
        public double AverageTime { get; set; }
        public double BestTime { get; set; }
        public double WorstTime { get; set; }
        public double AverageMemory { get; set; }
        public double BestMemory { get; set; }
        public double WorstMemory { get; set; }
        public double AverageCpu { get; set; }
        public double BestCpu { get; set; }
        public double WorstCpu { get; set; }
        public int ExecutionCount { get; set; } 
        public string TheoreticalBestCase { get; set; } 
        public string TheoreticalAverageCase { get; set; }
        public string TheoreticalWorstCase { get; set; }
        public int InvalidMeasurementCount { get; set; }
    }
}
