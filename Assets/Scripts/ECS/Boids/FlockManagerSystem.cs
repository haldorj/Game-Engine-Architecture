using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class FlockManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref FlockManagerComponent flockManager) =>
        {
            // Access the goalPos field of the FlockManagerComponent
            float3 goalPos = flockManager.goalPos;

            // You can now use goalPos in your logic
            if (UnityEngine.Random.Range(0, 10000) < 200)
            {
                goalPos = GetRandomGoalPos(ref flockManager);
                flockManager.goalPos = goalPos; // Update the goalPos field of the FlockManagerComponent
            }
        }).ScheduleParallel();
    }

    float3 GetRandomGoalPos(ref FlockManagerComponent flockManager)
    {
        float3 pos = new float3(
            UnityEngine.Random.Range(-flockManager.limits.x * 0.2f, flockManager.limits.x * 0.2f),
            UnityEngine.Random.Range(-flockManager.limits.y * 0.5f, flockManager.limits.y * 0.5f),
            UnityEngine.Random.Range(-flockManager.limits.z * 0.2f, flockManager.limits.z * 0.2f)
        );
        return pos;
    }
}