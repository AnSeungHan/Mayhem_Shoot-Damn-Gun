using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

partial struct System_RaycastingWall : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // DOTS 1.3에서는 PhysicsWorldSingleton을 사용
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;
        
        float3[] directions = new float3[]
        {
            new float3(1, 0, 0),
            new float3(-1, 0, 0),
            new float3(0, 0, 1),
            new float3(0, 0, -1),
            math.normalize(new float3(1, 0, 1)),
            math.normalize(new float3(-1, 0, 1)),
            math.normalize(new float3(1, 0, -1)),
            math.normalize(new float3(-1, 0, -1)),
        };

        foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<InputData>()
                     .WithEntityAccess())
        {
            float3 origin = transform.ValueRO.Position;
            float minDistance = float.MaxValue;
            float3 nearestHitPos = float3.zero;
            Entity hitEntity = Entity.Null;

            foreach (float3 dir in directions)
            {
                float3 rayEnd = origin + dir * 10f;
                
                var input = new RaycastInput
                {
                    Start = origin,
                    End = rayEnd,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = ~0u,
                        GroupIndex = 0
                    }
                };

                // 레이 시각화 (빨간색 - 충돌하지 않은 레이)
                Debug.DrawLine(origin, rayEnd, Color.red, 0.1f);

                if (collisionWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
                {
                    // 충돌 지점까지의 선 (초록색 - 충돌한 레이)
                    Debug.DrawLine(origin, hit.Position, Color.green, 0.1f);
                    
                    // 충돌 지점에 작은 십자 표시
                    float3 hitPos = hit.Position;
                    Debug.DrawLine(hitPos + new float3(0.1f, 0, 0), hitPos - new float3(0.1f, 0, 0), Color.yellow, 0.1f);
                    Debug.DrawLine(hitPos + new float3(0, 0.1f, 0), hitPos - new float3(0, 0.1f, 0), Color.yellow, 0.1f);
                    Debug.DrawLine(hitPos + new float3(0, 0, 0.1f), hitPos - new float3(0, 0, 0.1f), Color.yellow, 0.1f);
                    
                    float dist = math.distance(origin, hit.Position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestHitPos = hit.Position;
                        hitEntity = hit.Entity;
                    }
                }
            }

            if (minDistance < float.MaxValue)
            {
                // 가장 가까운 벽까지의 선을 굵게 표시 (파란색)
                Debug.DrawLine(origin, nearestHitPos, Color.blue, 0.2f);
                
                // 캐릭터 위치에 십자 표시 (마젠타색)
                Debug.DrawLine(origin + new float3(0.2f, 0, 0), origin - new float3(0.2f, 0, 0), Color.magenta, 0.2f);
                Debug.DrawLine(origin + new float3(0, 0.2f, 0), origin - new float3(0, 0.2f, 0), Color.magenta, 0.2f);
                Debug.DrawLine(origin + new float3(0, 0, 0.2f), origin - new float3(0, 0, 0.2f), Color.magenta, 0.2f);
                
                Debug.Log($"[{entity.Index}] 가장 가까운 벽 위치: {nearestHitPos}, 거리: {minDistance}, 충돌 엔티티: {hitEntity.Index}");
                // 필요한 추가 처리 위치
            }
        }
    }
}