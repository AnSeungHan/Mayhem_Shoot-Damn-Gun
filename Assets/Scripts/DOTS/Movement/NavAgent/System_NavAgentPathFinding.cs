using UnityEngine;

using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using static ConfigAuthoring;

[BurstCompile]
[UpdateAfter(typeof(System_FindNearTarget))]
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
        // Unmanaged에서는 NavMesh API를 직접 사용할 수 없으므로 managed job 사용
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var 
            (
                agent,
                target,
                transform,

                entity
            )
            in SystemAPI.Query
            <
                RefRW<NavAgentData>,
                RefRO<TargetData>,
                RefRO<LocalTransform>
            >()
            .WithEntityAccess())
        {
            if (Entity.Null == target.ValueRO.targetEntity)
                continue;

            float3 startPos = transform.ValueRO.Position;
            float3 endPos   = target.ValueRO.targetTransform.Position;
            float3 prevPos  = agent.ValueRW.preTargetPosition;

            float3 targetPos_Cur = new float3(endPos.x , 0f, endPos.z);
            float3 targetPos_Pos = new float3(prevPos.x, 0f, prevPos.z);
            float dist = math.distance(targetPos_Cur, targetPos_Pos);

            if (0.4f > dist &&
               agent.ValueRO.hasPath)
            {
                continue;
            }

            if (agent.ValueRO.pathBlob.IsCreated)
            {
                agent.ValueRW.pathBlob.Dispose();
            }

            // Unity NavMesh를 사용하여 경로 계산 (managed 코드)
            var path = new UnityEngine.AI.NavMeshPath();

            /*UnityEngine.AI.NavMeshTriangulation triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
            Debug.Log($"NavMesh vertices count: {triangulation.vertices.Length}");*/

            /*UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(startPos, out hit, 5f, -1))
            {
                startPos = hit.position;
            }

            if (UnityEngine.AI.NavMesh.SamplePosition(endPos, out hit, 5f, -1))
            {
                endPos = hit.position;
            }*/

            if (UnityEngine.AI.NavMesh.CalculatePath
                (
                    startPos, 
                    endPos, 
                    -1,
                    path
                ))
            {
                if (path.corners.Length > 0)
                {
                    // 새로운 Blob Asset 생성
                    using var builder = new BlobBuilder(Allocator.Temp);
                    ref var pathBlob  = ref builder.ConstructRoot<NavMeshPathBlob>();
                    var cornersArray  = builder.Allocate(ref pathBlob.Corners, path.corners.Length);

                    for (int i = 0; i < path.corners.Length; i++)
                    {
                        cornersArray[i] = path.corners[i];
                    }

                    var blobAsset = builder.CreateBlobAssetReference<NavMeshPathBlob>(Allocator.Persistent);

                    agent.ValueRW.pathBlob           = blobAsset;
                    agent.ValueRW.currentCornerIndex = 0;
                    agent.ValueRW.hasPath            = true;
                    agent.ValueRW.preTargetPosition  = endPos;
                }
            }
        }
        
    }
}
