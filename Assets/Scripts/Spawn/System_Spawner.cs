using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

using static ConfigAuthoring;

[BurstCompile]
public partial struct System_Spawner : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        foreach (var 
            (
                spawner,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<SpawnerData>
            >()
            .WithEntityAccess())
        {
            for (int i = 0; i < spawner.ValueRO.numCreate; ++i)
            { 
                var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.createPrefab);

                float3 pos = new float3
                (
                    UnityEngine.Random.Range(0f, spawner.ValueRO.bounds.x),
                    0f,
                    UnityEngine.Random.Range(0f, spawner.ValueRO.bounds.y)
                );

                state.EntityManager.SetComponentData(newEntity, new LocalTransform 
                {
                    Position    = pos,
                    Rotation    = quaternion.identity,
                    Scale       = 1f
                });
            }
        }
    }
}
