using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateAfter(typeof(System_FindNearTarget))]
public partial struct System_DirectionMovement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MovementDirectionData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new Job_DirectionMovement
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();
    }
}
