/// <summary>
/// This class will be used to store values that will be used all over the project,
/// such as chunk size, number of threads per workgroup (for the compute shader) etc.
/// </summary>

public static class GridMetrics 
{
    public const int NumThreads = 8;
    public const int Scale = 32;
    
    public const int GroundLevel = Scale / 2;
    
    // preset values of LODs (should be divisible by 8)
    private static readonly int[] LoDs = {
        8,
        16,
        24,
        32,
        40
    };
    
    public static readonly int LastLod = LoDs.Length - 1;
    
    // Point density per chunk
    public static int PointsPerChunk(int lod) {
        return LoDs[lod];
    }

    // number of threadgroups
    public static int ThreadGroups(int lod) {
        return LoDs[lod] / NumThreads;
    }
}