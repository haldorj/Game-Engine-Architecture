using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Chunk : MonoBehaviour
{
    public ComputeShader marchingShader;
    public MeshFilter meshFilter;

    // Buffers to transfer data between the CPU and the GPU
    private ComputeBuffer _trianglesBuffer;
    private ComputeBuffer _trianglesCountBuffer;
    private ComputeBuffer _weightsBuffer;
    
    private float[] _weights;

    [FormerlySerializedAs("NoiseGenerator")] public NoiseGenerator noiseGenerator;
    private static readonly int Triangles = Shader.PropertyToID("triangles");
    private static readonly int Weights = Shader.PropertyToID("weights");
    private static readonly int ChunkSize = Shader.PropertyToID("chunk_size");
    private static readonly int IsoLevel = Shader.PropertyToID("iso_level");

    private void Awake() 
    {
        meshFilter = GetComponent<MeshFilter>();
        CreateBuffers();
    }

    private void OnDestroy()
    {
        ReleaseBuffers();
    }

    private struct Triangle {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        // Every triangle is the size of 3 * 3 floats (three points of xyz coords).
        public static int SizeOf => sizeof(float) * 3 * 3;
    }

    void Start() {
        _weights = noiseGenerator.GetNoise();

        meshFilter.sharedMesh = ConstructMesh();
    }

    Mesh ConstructMesh() {
        marchingShader.SetBuffer(0, Triangles, _trianglesBuffer);
        marchingShader.SetBuffer(0, Weights, _weightsBuffer);

        marchingShader.SetInt(ChunkSize, GridMetrics.PointsPerChunk);
        marchingShader.SetFloat(IsoLevel, .5f);

        _weightsBuffer.SetData(_weights);
        _trianglesBuffer.SetCounterValue(0);
        
        marchingShader.Dispatch(0, 
            GridMetrics.PointsPerChunk / GridMetrics.NumThreads, 
            GridMetrics.PointsPerChunk / GridMetrics.NumThreads, 
            GridMetrics.PointsPerChunk / GridMetrics.NumThreads);

        Triangle[] triangles = new Triangle[ReadTriangleCount()];
        _trianglesBuffer.GetData(triangles);

        return CreateMeshFromTriangles(triangles);
    }

    int ReadTriangleCount() {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        _trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    Mesh CreateMeshFromTriangles(Triangle[] triangles) 
    {
        Vector3[] verts = new Vector3[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++) 
        {
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
    
    void CreateBuffers() 
    {
        _trianglesBuffer = new ComputeBuffer(
            5 * (GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk), 
            Triangle.SizeOf, ComputeBufferType.Append
            );
        
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        
        _weightsBuffer = new ComputeBuffer(
            GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk, 
            sizeof(float)
            );
    }

    void ReleaseBuffers() 
    {
        _trianglesBuffer.Release();
        _trianglesCountBuffer.Release();
        _weightsBuffer.Release();
    }
    
    private void OnDrawGizmos() 
    {
        if (_weights == null || _weights.Length == 0) 
        {
            return;
        }
        for (int x = 0; x < GridMetrics.PointsPerChunk; x++) 
        {
            for (int y = 0; y < GridMetrics.PointsPerChunk; y++) 
            {
                for (int z = 0; z < GridMetrics.PointsPerChunk; z++) 
                {
                    int index = x + GridMetrics.PointsPerChunk * (y + GridMetrics.PointsPerChunk * z);
                    float noiseValue = _weights[index];
                    Gizmos.color = Color.Lerp(Color.black, Color.white, noiseValue);
                    Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * .2f);
                }
            }
        }
    }
}
