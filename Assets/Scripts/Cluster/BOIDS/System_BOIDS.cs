using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

using static ConfigAuthoring;
using Unity.Collections;

[BurstCompile]
[UpdateAfter(typeof(System_DirectionMovement))]
[UpdateAfter(typeof(System_NavAgentMove))]
public partial struct System_BOIDS : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_Cluster_BOIDS>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<LocalToWorld>()
            .Build();

        var allTransforms = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var job = new Job_BOIDS
        {
            DeltaTime       = SystemAPI.Time.DeltaTime,
            AllTransforms   = allTransforms
        };

        job.ScheduleParallel();
    }
}
