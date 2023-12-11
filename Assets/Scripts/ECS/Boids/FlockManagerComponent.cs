using Unity.Entities;
using Unity.Mathematics;

public struct FlockManagerComponent : IComponentData
{
    public int numBoids;
    public float3 limits;
    public float3 goalPos;
    public float minSpeed;
    public float maxSpeed;
    public float neighbourDistance;
    public float rotationSpeed;
}