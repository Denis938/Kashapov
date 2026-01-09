namespace Lab2.Services;

public class BenchmarkResult
{
    public string CollectionType { get; set; }
    public string Operation { get; set; }
    public double AverageTimeMs { get; set; }
    public int ElementCount { get; set; }
    public int Iterations { get; set; }

    public override string ToString()
    {
        return $"{CollectionType,-20} | {Operation,-25} | {AverageTimeMs,10:F4} ms";
    }
}