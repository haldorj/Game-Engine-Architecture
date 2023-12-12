using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class NoiseGenerator : MonoBehaviour
{
    /// <summary>
    /// this buffer is responsible for communicating the noise values
    /// generated by the GPU compute shader back to the CPU C# script.
    /// </summary>
    private ComputeBuffer _weightsBuffer;
    
    public ComputeShader noiseShader;
    
    public int initialX;
    public int initialY;
    public int initialZ;
    
    [SerializeField] private float amplitude = 10;
    [SerializeField] private float frequency = 0.002f;
    [SerializeField] private int octaves = 8;
    [SerializeField] private int hardFloor = 2;
    [SerializeField] private int terraceHeight = 6;
    [SerializeField, Range(0f, 1f)] private float groundPercent = 0.2f;
    
    public float[] GetNoise(int lod)
    {
        CreateBuffers(lod);
        // Array of floats with size equal to the size of the 3d grid
        float[] noiseValues = 
            new float[GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod)];
        
        // Communicate between GPU and CPU (compute shader to C# script)
        noiseShader.SetBuffer(0, $"weights", _weightsBuffer);
        
        noiseShader.SetInt($"chunk_size", GridMetrics.PointsPerChunk(lod));
        noiseShader.SetFloat($"amplitude", amplitude);
        noiseShader.SetFloat($"frequency", frequency);
        noiseShader.SetInt($"octaves", octaves);
        noiseShader.SetFloat($"ground_percent", groundPercent);
        noiseShader.SetInt($"hard_floor", hardFloor);
        noiseShader.SetInt($"terrace_height", terraceHeight);
        noiseShader.SetInt($"scale", GridMetrics.Scale);
        noiseShader.SetInt($"ground_level", GridMetrics.GroundLevel);
        noiseShader.SetInt($"initial_x", initialX);
        noiseShader.SetInt($"initial_y", initialY);
        noiseShader.SetInt($"initial_z", initialZ);
        
        // Dispatch shader, with one kernel (index 0), and
        // (GridMetrics.PointsPerChunk/GridMetrics.NumThreads)
        // workgroups for each dimension to create 3d a grid
        noiseShader.Dispatch(
            0, 
            GridMetrics.ThreadGroups(lod), 
            GridMetrics.ThreadGroups(lod), 
            GridMetrics.ThreadGroups(lod)
        );
        
        // Transfer the data on the buffer back to the noiseValues array.
        _weightsBuffer.GetData(noiseValues);
        
        ReleaseBuffers();
        return noiseValues;
    }

    void CreateBuffers(int lod) {
        _weightsBuffer = new ComputeBuffer(
            GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod), 
            sizeof(float)
        );
    }

    void ReleaseBuffers() {
        _weightsBuffer.Release();
    }
}
