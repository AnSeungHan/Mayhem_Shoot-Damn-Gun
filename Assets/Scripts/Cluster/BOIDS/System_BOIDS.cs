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

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        Vector3[] vertices = triangulation.vertices;
        int[] indices = triangulation.indices;

        NativeArray<float3> navMeshVerts = new NativeArray<float3>(vertices.Length, Allocator.TempJob);
        NativeArray<int> navMeshTris = new NativeArray<int>(indices.Length, Allocator.TempJob);

        for (int i = 0; i < vertices.Length; i++)
            navMeshVerts[i] = vertices[i];

        for (int i = 0; i < indices.Length; i++)
            navMeshTris[i] = indices[i];

        var allTransforms = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var job = new Job_BOIDS
        {
            DeltaTime       = SystemAPI.Time.DeltaTime,
            AllTransforms   = allTransforms,
            NavMeshVertices = navMeshVerts,
            NavMeshIndices  = navMeshTris
        };

        job.ScheduleParallel();
    }
}
