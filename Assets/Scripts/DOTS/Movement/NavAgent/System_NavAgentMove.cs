using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(System_NavAgentPathFinding))]
public partial struct System_NavAgentMove : ISystem
{
    private EntityQuery _pathfindingQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_Movement_NavAgent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float DeltaTime = SystemAPI.Time.DeltaTime;

        var query = SystemAPI.QueryBuilder()
            .WithAll<LocalToWorld>()
            .Build();

        var allTransforms = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

        var job = new Job_NavAgentMove
        {
            DeltaTime       = DeltaTime,
            AllTransforms   = allTransforms
        };

        var handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
        handle.Complete();

        allTransforms.Dispose();
    }
}
