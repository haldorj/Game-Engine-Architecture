/// <summary>
/// This class will be used to store values that will be used all over the project,
/// such as chunk size, number of threads per workgroup (for the compute shader) etc.
/// </summary>

public static class GridMetrics 
{
    public const int NumThreads = 8;
    public const int PointsPerChunk = 16;
}