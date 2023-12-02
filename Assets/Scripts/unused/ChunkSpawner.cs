using Unity.VisualScripting;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public Chunk chunk; // Reference to your chunk prefab
    public NoiseGenerator noiseGenerator;
    private const int Iterations = 3; // Number of iterations for the grid
    private readonly float _distance = GridMetrics.Scale;  // Distance between chunks

    void Start()
    {
        SpawnChunks();
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
