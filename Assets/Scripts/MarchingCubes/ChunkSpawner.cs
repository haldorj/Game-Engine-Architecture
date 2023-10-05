using Unity.VisualScripting;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public Chunk chunk; // Reference to your chunk prefab
    private int _iterations = 1;     // Number of iterations for the grid
    private float _distance = GridMetrics.Scale - 1;  // Distance between chunks

    void Start()
    {
        SpawnChunks();
    }

    void SpawnChunks()
    {
        for (int x = -_iterations; x <= _iterations; x++)
        {
            for (int y = -_iterations; y <= _iterations; y++)
            {
                Vector3 spawnPosition = new Vector3(x * _distance, 0.0f, y * _distance);
                
                Quaternion spawnRotation = Quaternion.Euler(0,0,0);

                GameObject spawnedChunk = Instantiate(chunk.GameObject(), 
                    spawnPosition, spawnRotation);

                if (spawnedChunk != null)
                {
                    spawnedChunk.GetComponent<Chunk>()._initialX = x;
                    spawnedChunk.GetComponent<Chunk>()._initialY = y;
                }
            }
        }
    }
}
