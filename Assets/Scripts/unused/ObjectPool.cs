// using System.Collections.Generic;
// using UnityEngine;
//
// public class ObjectPool : MonoBehaviour
// {
//     [SerializeField] private uint initPoolSize;
//     [SerializeField] private Chunk chunkPrefab;
//     [SerializeField] private NoiseGenerator noiseGeneratorPrefab;
//     
//     private const int Iterations = 6; // Number of iterations for the grid
//     private readonly float _distance = GridMetrics.Scale; // Distance between chunks
//     public Transform playerTransform;
//     
//     private Stack<Chunk> _chunkPool;
//
//     private void Start()
//     {
//         SetupPool();
//         //SetupChunkPool();
//
//     }
//
//     void SetupChunkPool()
//     {
//         _chunkPool = new Stack<Chunk>();
//         for (int y = 0; y <= 1; y++)
//         {
//             for (int x = -Iterations; x <= Iterations; x++)
//             {
//                 for (int z = -Iterations; z <= Iterations; z++)
//                 {
//                     // Get a chunk from the pool
//                     Chunk chunkInstance = Instantiate(chunkPrefab);
//
//                     Vector3 spawnPosition = new Vector3(x * _distance, y * _distance, z * _distance);
//                     Quaternion spawnRotation = Quaternion.identity;
//
//                     chunkInstance.transform.position = spawnPosition;
//                     chunkInstance.transform.rotation = spawnRotation;
//
//                     // Set up noise generator
//                     NoiseGenerator noiseInstance = Instantiate(noiseGeneratorPrefab, spawnPosition, spawnRotation);
//                     noiseInstance.initialX = x;
//                     noiseInstance.initialY = y;
//                     noiseInstance.initialZ = z;
//                     chunkInstance.noiseGenerator = noiseInstance;
//
//                     // Parenting
//                     noiseInstance.transform.parent = chunkInstance.transform;
//
//                     chunkInstance.Pool = this;
//                     chunkInstance.gameObject.SetActive(false);
//                     _chunkPool.Push(chunkInstance);
//                 }
//             }
//         }
//     }
//
//     private void SetupPool()
//     {
//         _chunkPool = new Stack<Chunk>();
//         for (int i = 0; i < initPoolSize; i++)
//         {
//             Chunk instance = Instantiate(chunkPrefab);
//             instance.Pool = this;
//             instance.gameObject.SetActive(false);
//             _chunkPool.Push(instance);
//         }
//     }
//
//     // Returns the first active GameObject from the pool
//     public Chunk GetPooledObject()
//     {
//         // If the pool is not large enough, instantiate a new Chunk
//         if (_chunkPool.Count == 0)
//         {
//             Chunk newInstance = Instantiate(chunkPrefab);
//             newInstance.Pool = this;
//             return newInstance;
//         }
//
//         // Otherwise, just grab the next one from the list
//         Chunk nextInstance = _chunkPool.Pop();
//         nextInstance.gameObject.SetActive(true);
//         return nextInstance;
//     }
//
//     public void ReturnToPool(Chunk pooledObject)
//     {
//         _chunkPool.Push(pooledObject);
//         pooledObject.gameObject.SetActive(false);
//     }
//     
//     private void Update()
//     {
//         if (Time.frameCount % 3 == 0)
//         {
//             UpdateChunks();
//             UpdateChunkLOD();
//         }
//     }
//
//     private void UpdateChunks()
//     {
//         foreach (Chunk chunk in _chunkPool)
//         {
//             // Calculate the distance between the chunk and the player
//             float distance = Vector3.Distance(chunk.transform.position, playerTransform.position);
//             Vector3 distanceVector = chunk.transform.position - playerTransform.position;
//
//             // Check if the chunk should be active based on distance
//             bool shouldBeActive = distance <= 150;
//
//             // If the chunk is not in the correct active state, update it
//             if (chunk.gameObject.activeSelf != shouldBeActive)
//             {
//                 if (shouldBeActive)
//                 {
//                     // Activate the chunk and set its position
//                     // Use GetPooledObject to get a new or inactive chunk from the pool
//                     //Chunk newChunk = GetPooledObject();
//                     chunk.transform.position = CalculateNewChunkPosition(distanceVector);
//                     chunk.gameObject.SetActive(true);
//                 }
//                 else
//                 {
//                     // Deactivate the chunk and return it to the pool
//                     chunk.gameObject.SetActive(false);
//                     ReturnToPool(chunk);
//                 }
//             }
//         }
//     }
//     
//     Vector3 CalculateNewChunkPosition(Vector3 distanceVector)
//     {
//         // Calculate the new position of the chunk
//         Vector3 newPosition = playerTransform.position + distanceVector.normalized * _distance;
//         return newPosition;
//     }
//
//     void UpdateChunkLOD()
//     {
//         foreach (Chunk chunk in _chunkPool)
//         {
//             float distance = Vector3.Distance(chunk.transform.position, playerTransform.position);
//
//             if (distance < 150)
//             {
//                 chunk.lod = 5;
//             }
//             else if (distance > 150 && distance < 200)
//             {
//                 chunk.lod = 4;
//             }
//             else if (distance > 200 && distance < 250)
//             {
//                 chunk.lod = 3;
//             }
//             else if (distance > 250 && distance < 300)
//             {
//                 chunk.lod = 2;
//             }
//             else if (distance > 300 && distance < 350)
//             {
//                 chunk.lod = 1;
//             }
//             else if (distance > 350)
//             {
//                 chunk.lod = 0;
//             }
//         }
//     }
// }
