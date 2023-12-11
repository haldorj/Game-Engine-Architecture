// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// public partial class BoidSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         float deltaTime = UnityEngine.Time.deltaTime;
//         float3 goalPos = float3.zero;
//
//         Entities.ForEach((ref Translation translation, ref Rotation rotation, ref BoidComponent boid) =>
//         {
//             // Access the goalPos field of the FlockManagerComponent
//             goalPos = boid.goalPos;
//
//             // You can now use goalPos in your logic
//             if (UnityEngine.Random.Range(0, 10000) < 200)
//             {
//                 goalPos = GetRandomGoalPos(ref boid);
//                 boid.goalPos = goalPos; // Update the goalPos field of the FlockManagerComponent
//             }
//         }).ScheduleParallel();
//     }
// }