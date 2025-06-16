using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public partial struct Job_ConvertNavMeshData : IJob
{
    [ReadOnly]
    public Vector3[] Vertices;
    [ReadOnly]
    public int[] Indices;

    public NativeArray<float3> NavMeshVerts;
    public NativeArray<int>    NavMeshTris;

    public void Execute()
    {
        for (int i = 0; i < Vertices.Length; i++)
            NavMeshVerts[i] = Vertices[i];

        for (int i = 0; i < Indices.Length; i++)
            NavMeshTris[i] = Indices[i];
    }
}
