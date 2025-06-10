using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateAfter(typeof(System_FindNearTarget))]
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

    }
}
