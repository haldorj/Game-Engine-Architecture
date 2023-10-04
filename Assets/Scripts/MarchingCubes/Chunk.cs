using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Chunk : MonoBehaviour
{
    public ComputeShader marchingShader;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    private Mesh _mesh;

    // Buffers to transfer data between the CPU and the GPU
    private ComputeBuffer _trianglesBuffer;
    private ComputeBuffer _trianglesCountBuffer;
    private ComputeBuffer _weightsBuffer;
    
    private float[] _weights;

    [FormerlySerializedAs("NoiseGenerator")] public NoiseGenerator noiseGenerator;
    
    #region Shader properties
    
    private static readonly int Triangles = Shader.PropertyToID("triangles");
    private static readonly int Weights = Shader.PropertyToID("weights");
    private static readonly int ChunkSize = Shader.PropertyToID("chunk_size");
    private static readonly int IsoLevel = Shader.PropertyToID("iso_level");
    private static readonly int HitPosition = Shader.PropertyToID("hit_position");
    private static readonly int BrushSize = Shader.PropertyToID("brush_size");
    private static readonly int TerraformStrength = Shader.PropertyToID("terraform_strength");
    private static readonly int Scale = Shader.PropertyToID("scale");
    
    #endregion

    [FormerlySerializedAs("LOD")] [Range(0, 4)]
    public int lod;

   

    private void Awake() 
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        
    }

    private struct Triangle {
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

    private void Create()
    {
        CreateBuffers();
        
        _weights = noiseGenerator.GetNoise(lod);
        _mesh = new Mesh();
        //meshFilter.sharedMesh = ConstructMesh();
        UpdateMesh();
        
        ReleaseBuffers();
    }

    private Mesh ConstructMesh()
    {
        int kernel = marchingShader.FindKernel("march");
        
        marchingShader.SetBuffer(0, Triangles, _trianglesBuffer);
        marchingShader.SetBuffer(0, Weights, _weightsBuffer);
        
        marchingShader.SetInt(ChunkSize, GridMetrics.PointsPerChunk(lod));
        marchingShader.SetFloat(IsoLevel, .5f);
        marchingShader.SetInt(Scale, GridMetrics.Scale);

        _weightsBuffer.SetData(_weights);
        _trianglesBuffer.SetCounterValue(0);
        
        marchingShader.Dispatch(
            kernel, 
            GridMetrics.ThreadGroups(lod), 
            GridMetrics.ThreadGroups(lod), 
            GridMetrics.ThreadGroups(lod)
            );

        Triangle[] triangles = new Triangle[ReadTriangleCount()];
        _trianglesBuffer.GetData(triangles);

        return CreateMeshFromTriangles(triangles);
    }
    
    public void EditWeights(Vector3 hitPosition, float brushSize, bool add)
    {
        CreateBuffers();
        // We are using the marching compute shader to update the weights for the mesh (terraforming).
        // We are using a new kernel (method/function) in our compute shader to do this.
        // With the .FindKernel("name") method we can find the index of the kernel in our compute shader.
        int kernel = marchingShader.FindKernel("update_weights");
        
        _weightsBuffer.SetData(_weights);
        marchingShader.SetBuffer(kernel, Weights, _weightsBuffer);
        
        marchingShader.SetInt(ChunkSize, GridMetrics.PointsPerChunk(lod));
        marchingShader.SetVector(HitPosition, hitPosition);
        marchingShader.SetFloat(BrushSize, brushSize);
        marchingShader.SetFloat(TerraformStrength, add ? 1f : -1f);
        
        marchingShader.Dispatch(
            kernel, 
            GridMetrics.ThreadGroups(lod),
            GridMetrics.ThreadGroups(lod),
            GridMetrics.ThreadGroups(lod)
            );

        _weightsBuffer.GetData(_weights);
        UpdateMesh();
        
        ReleaseBuffers();
    }

    private int ReadTriangleCount() {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        _trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    private Mesh CreateMeshFromTriangles(IList<Triangle> triangles) 
    {
        Vector3[] verts = new Vector3[triangles.Count * 3];
        int[] tris = new int[triangles.Count * 3];

        for (int i = 0; i < triangles.Count; i++) 
        {
            int startIndex = i * 3;

            verts[startIndex] = triangles[i].A;
            verts[startIndex + 1] = triangles[i].B;
            verts[startIndex + 2] = triangles[i].C;

            tris[startIndex] = startIndex;
            tris[startIndex + 1] = startIndex + 1;
            tris[startIndex + 2] = startIndex + 2;
        }

        // Mesh mesh = new Mesh();
        // mesh.vertices = verts;
        // mesh.triangles = tris;
        // mesh.RecalculateNormals();
        
        _mesh.Clear();
        _mesh.vertices = verts;
        _mesh.triangles = tris;
        _mesh.RecalculateNormals();
        
        return _mesh;
    }

    private void UpdateMesh()
    {
        Mesh mesh = ConstructMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void CreateBuffers() 
    {
        _trianglesBuffer = new ComputeBuffer(
            5 * (GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod)), 
            Triangle.SizeOf, ComputeBufferType.Append
            );
        
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        
        _weightsBuffer = new ComputeBuffer(
            GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod), 
            sizeof(float)
            );
    }

    private void ReleaseBuffers() 
    {
        _trianglesBuffer.Release();
        _trianglesCountBuffer.Release();
        _weightsBuffer.Release();
    }
    
    private void OnValidate() 
    {
        if (Application.isPlaying) 
        {
            Create();
        }
    }
    
    private void OnDrawGizmos() 
    {
        /*
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
        */
    }
}
