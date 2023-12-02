using System;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public Chunk chunk; // Reference to your chunk prefab
    public NoiseGenerator noiseGenerator;
    private const int Iterations = 12; // Number of iterations for the grid
    private readonly float _distance = GridMetrics.Scale;  // Distance between chunks

    [SerializeField] private Transform playerTransform;

    private int _interval = 3;
    
    void Start()
    {
        SpawnChunks();
    }

    private void Update()
    {
        if (Time.frameCount % _interval == 0)
        {
            UpdateChunks();
            UpdateChunkLOD();
        }
    }

    private void UpdateChunks()
    {
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(child.position, playerTransform.position);
            if (distance > 400)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    void UpdateChunkLOD()
    {
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(child.position, playerTransform.position);
            
            var chunk = child.gameObject.GetComponent<Chunk>();
            
            if (distance < 150)
            {
                chunk.lod = 5;
            }
            else
            if (distance > 150 && distance < 200)
            {
                chunk.lod = 4;
            }
            else if (distance > 200 && distance < 250)
            {
                chunk.lod = 3;
            }
            else if (distance > 250 && distance < 300)
            {
                chunk.lod = 2;
            }
            else if (distance > 300 && distance < 350)
            {
                chunk.lod = 1;
            }
            else if (distance > 350)
            {
                chunk.lod = 0;
            }
        }
    }

    void SpawnChunks()
    {
        for (int x = -Iterations; x <= Iterations; x++)
        {
            for (int z = -Iterations; z <= Iterations; z++)
            {
                Vector3 spawnPosition = new Vector3(x * _distance, 0.0f, z * _distance);
                
                Quaternion spawnRotation = Quaternion.identity;

                noiseGenerator.initialX = x;
                noiseGenerator.initialZ = z;
                
                GameObject spawnedNoise = Instantiate(noiseGenerator.gameObject, 
                    spawnPosition, spawnRotation);
                
                
                chunk.noiseGenerator = spawnedNoise.GetComponent<NoiseGenerator>();
                GameObject spawnedChunk = Instantiate(chunk.GameObject(), 
                    spawnPosition, spawnRotation);
                
                spawnedChunk.transform.parent = transform;
                spawnedNoise.transform.parent = spawnedChunk.transform;
            }
        }
    }
    
    
}
