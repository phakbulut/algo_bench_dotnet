namespace algo.Data
{
    // /Models/AlgorithmLog.cs
    public class AlgorithmLog
    {
        public int Id { get; set; }
        public string? Algorithm { get; set; }
        public string? ArrayType { get; set; }
        public int ArraySize { get; set; }
        public double TimeTaken { get; set; }
        public double AverageMemory { get; set; }
        public double AverageCpu { get; set; }
    }

}
