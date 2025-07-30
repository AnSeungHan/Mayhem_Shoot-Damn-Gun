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
                transform,
                spawner,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRO<SpawnerData>
            >()
            .WithEntityAccess())
        {
            float2 center   = transform.ValueRO.Position.xz;
            float2 halfSize = spawner.ValueRO.bounds * 0.5f;
            float2 begin    = center - halfSize;
            float2 end      = center + halfSize;

            for (int i = 0; i < spawner.ValueRO.numCreate; ++i)
            { 
                var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.createPrefab);

                float3 pos = new float3
                (
                    UnityEngine.Random.Range(begin.x, end.x),
                    0f,
                    UnityEngine.Random.Range(begin.y, end.y)
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
