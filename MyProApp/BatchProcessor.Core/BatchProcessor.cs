namespace BatchProcessor.Core
{
    public class BatchProcessor
    {
        public int MaxBatchSize { get; set; } = 4;
        public int TimeoutSeconds { get; set; } = 2;

        public void ProcessData()
        {
            // Simulated batch processing logic
            Console.WriteLine("Processing data...");
        }
    }
}
