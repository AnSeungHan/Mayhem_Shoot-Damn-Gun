using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

using static ConfigAuthoring;

[BurstCompile]
partial struct System_Create : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerData>();
        state.RequireForUpdate<ButtonClickState>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

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

                float2 begin = spawner.ValueRO.boundsBegin;
                float2 end   = spawner.ValueRO.boundsEnd;

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

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var 
            (
                buttonClickState,
                entity
            ) 
            in SystemAPI.Query
            <
                RefRO<ButtonClickState>
            >()
            .WithEntityAccess())
        {
            // 조건에 따라 삭제
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
