using UnityEngine;
using UnityEngine.AI;

using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

using static ConfigAuthoring;

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

        // 네비 메쉬 정점 정보
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        NativeArray<float3> navMeshVerts = new NativeArray<float3>(vertices.Length, Allocator.TempJob);
        NativeArray<int>    navMeshTris  = new NativeArray<int>(indices.Length, Allocator.TempJob);
        
        var convertJob = new Job_ConvertNavMeshData
        {
            Vertices     = triangulation.vertices,
            Indices      = triangulation.indices,
            
            NavMeshVerts = navMeshVerts,
            NavMeshTris  = navMeshTris
        };
        
        JobHandle convertHandle = convertJob.Schedule();
        
        // BOIDS 알고리즘
        var allTransforms = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var job = new Job_BOIDS
        {
            DeltaTime       = SystemAPI.Time.DeltaTime,
            AllTransforms   = allTransforms,
            NavMeshVertices = navMeshVerts,
            NavMeshIndices  = navMeshTris
        };

        job.ScheduleParallel(convertJob);

        navMeshVerts.Dispose();
        navMeshTris.Dispose();
        allTransforms.Dispose();
    }
}
