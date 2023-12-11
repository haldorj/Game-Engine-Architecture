using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class FlockManagerAuthoring : MonoBehaviour
{
    public int numBoids = 100;
    public float3 limits = new float3(50f, 50f, 50f);
    public float minSpeed = 1f;
    public float maxSpeed = 2f;
    public float neighbourDistance = 2f;
    public float rotationSpeed = 1f;

    public void Convert(Entity entity, EntityManager dstManager)
    {
        dstManager.AddComponentData(entity, new FlockManagerComponent
        {
            numBoids = numBoids,
            limits = limits,
            goalPos = float3.zero,
            minSpeed = minSpeed,
            maxSpeed = maxSpeed,
            neighbourDistance = neighbourDistance,
            rotationSpeed = rotationSpeed
        });
    }
}