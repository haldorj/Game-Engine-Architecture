using UnityEngine;
using Unity.Entities;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate;
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            spawnPosition = authoring.transform.position,
            nextSpawnTime = 0.0f,
            spawnRate = authoring.spawnRate
        });
    }
}