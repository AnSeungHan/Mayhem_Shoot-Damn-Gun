using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
public partial struct System_DirectionMovement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_Movement_Direction>();
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
