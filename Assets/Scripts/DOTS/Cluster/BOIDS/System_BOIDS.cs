using UnityEngine;
using UnityEngine.AI;

using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

using static ConfigAuthoring;

[UpdateAfter(typeof(System_DirectionMovement))]
[UpdateAfter(typeof(System_NavAgentMove))]
public partial struct System_BOIDS : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_Cluster_BOIDS>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<LocalToWorld>()
            .Build();

        // 네비 메쉬 정점 정보
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        int vertexCount = triangulation.vertices.Length;
        int indexCount = triangulation.indices.Length;
        NativeArray<float3> navMeshVerts = new NativeArray<float3>(vertexCount, Allocator.TempJob);
        NativeArray<int>    navMeshTris  = new NativeArray<int>(indexCount, Allocator.TempJob);

        for (int i = 0; i < vertexCount; i++)
            navMeshVerts[i] = triangulation.vertices[i];

        for (int i = 0; i < indexCount; i++)
            navMeshTris[i] = triangulation.indices[i];

        // BOIDS 알고리즘
        var allTransforms = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var job = new Job_BOIDS
        {
            DeltaTime       = SystemAPI.Time.DeltaTime,
            AllTransforms   = allTransforms,
            NavMeshVertices = navMeshVerts,
            NavMeshIndices  = navMeshTris
        };

        var handle = job.ScheduleParallel(state.Dependency);
        handle.Complete();

        navMeshVerts.Dispose();
        navMeshTris.Dispose();
        allTransforms.Dispose();
    }
}
