using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateAfter(typeof(System_DirectionMovement))]
[UpdateAfter(typeof(System_NavAgentMove))]
[UpdateAfter(typeof(System_BOIDS))]
public partial struct System_Movement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<MovementData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new Job_Movement()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();
    }
}
