using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public ComputeShader marchingShader;
    
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    // Buffers to transfer data between the CPU and the GPU
    private ComputeBuffer _trianglesBuffer;
    private ComputeBuffer _trianglesCountBuffer;
    private ComputeBuffer _weightsBuffer;
    
    private float[] _weights;

    public NoiseGenerator noiseGenerator;
    
    [Range(0, 4)]
    public int lod;
    
    private struct Triangle 
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        // Every triangle is the size of 3 * 3 floats (three points of xyz coords).
        public static int SizeOf => sizeof(float) * 3 * 3;
    }
    
    private void Start() 
    {
        Create();
    }
    
    // private void OnValidate() 
    // {
    //     if (Application.isPlaying) 
    //     {
    //         Create();
    //     }
    // }

    private void Create()
    {
        CreateBuffers();
        _weights ??= noiseGenerator.GetNoise(GridMetrics.LastLod);
        UpdateMesh();
        ReleaseBuffers();
    }
    
    private void UpdateMesh()
    {
        Mesh mesh = ConstructMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void EditWeights(Vector3 hitPosition, float brushSize, bool add)
    {
        CreateBuffers();
        // We are using the marching compute shader to update the weights for the mesh (terraforming).
        // We are using a new kernel (method/function) in our compute shader to do this.
        // With the .FindKernel("name") method we can find the index of the kernel in our compute shader.
        int kernel = marchingShader.FindKernel("update_weights");
        
        _weightsBuffer.SetData(_weights);
        marchingShader.SetBuffer(kernel, $"weights", _weightsBuffer);

        marchingShader.SetInt($"chunk_size", GridMetrics.PointsPerChunk(GridMetrics.LastLod));
        marchingShader.SetVector($"hit_position", hitPosition);
        marchingShader.SetFloat($"brush_size", brushSize);
        marchingShader.SetFloat($"terraform_strength", add ? 1f : -1f);
        marchingShader.SetInt($"scale", GridMetrics.Scale);

        marchingShader.Dispatch(
            kernel, 
            GridMetrics.ThreadGroups(GridMetrics.LastLod), 
            GridMetrics.ThreadGroups(GridMetrics.LastLod), 
            GridMetrics.ThreadGroups(GridMetrics.LastLod)
        );

        _weightsBuffer.GetData(_weights);

        UpdateMesh();
        ReleaseBuffers();
    }
    
    private Mesh ConstructMesh()
    {
        int kernel = marchingShader.FindKernel("march");

        marchingShader.SetBuffer(kernel, $"triangles", _trianglesBuffer);
        marchingShader.SetBuffer(kernel, $"weights", _weightsBuffer);

        float lodScaleFactor = GridMetrics.PointsPerChunk(GridMetrics.LastLod) / (float)GridMetrics.PointsPerChunk(lod);

        marchingShader.SetFloat($"lod_scale_factor", lodScaleFactor);

        marchingShader.SetInt($"chunk_size", GridMetrics.PointsPerChunk(GridMetrics.LastLod));
        marchingShader.SetInt($"lod_size", GridMetrics.PointsPerChunk(lod));
        marchingShader.SetFloat($"iso_level", .5f);
        marchingShader.SetInt($"scale", GridMetrics.Scale);

        _weightsBuffer.SetData(_weights);
        _trianglesBuffer.SetCounterValue(0);
        
        marchingShader.Dispatch(kernel, GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod));

        Triangle[] triangles = new Triangle[ReadTriangleCount()];
        _trianglesBuffer.GetData(triangles);

        return CreateMeshFromTriangles(triangles);
    }
    
    private int ReadTriangleCount() 
    {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        _trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    private static Mesh CreateMeshFromTriangles(Triangle[] triangles) 
    {
        Vector3[] verts = new Vector3[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++) {
            int startIndex = i * 3;

            verts[startIndex] = triangles[i].A;
            verts[startIndex + 1] = triangles[i].B;
            verts[startIndex + 2] = triangles[i].C;

            tris[startIndex] = startIndex;
            tris[startIndex + 1] = startIndex + 1;
            tris[startIndex + 2] = startIndex + 2;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
    
    private void CreateBuffers() 
    {
        _trianglesBuffer = new ComputeBuffer(
            5 * (GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod)), 
            Triangle.SizeOf, ComputeBufferType.Append
            );
        
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        
        _weightsBuffer = new ComputeBuffer(
            GridMetrics.PointsPerChunk(GridMetrics.LastLod) * 
            GridMetrics.PointsPerChunk(GridMetrics.LastLod) * 
            GridMetrics.PointsPerChunk(GridMetrics.LastLod), 
            sizeof(float)
            );
    }

    private void ReleaseBuffers() 
    {
        _trianglesBuffer.Release();
        _trianglesCountBuffer.Release();
        _weightsBuffer.Release();
    }
}
