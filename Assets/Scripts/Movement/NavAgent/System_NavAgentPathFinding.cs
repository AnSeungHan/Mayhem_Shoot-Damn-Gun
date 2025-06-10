using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct System_NavAgentPathFinding : ISystem
{
    private EntityQuery _pathfindingQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_Movement_NavAgent>();

        _pathfindingQuery = SystemAPI.QueryBuilder()
            .WithAll<NavAgentData, TargetData, LocalTransform>()
            .Build();
    }
    public void OnUpdate(ref SystemState state)
    {
        
    }
}
