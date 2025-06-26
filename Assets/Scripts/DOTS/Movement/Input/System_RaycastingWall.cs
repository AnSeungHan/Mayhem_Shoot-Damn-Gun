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
        // DOTS 1.3������ PhysicsWorldSingleton�� ���
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

                // ���� �ð�ȭ (������ - �浹���� ���� ����)
                Debug.DrawLine(origin, rayEnd, Color.red, 0.1f);

                if (collisionWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
                {
                    // �浹 ���������� �� (�ʷϻ� - �浹�� ����)
                    Debug.DrawLine(origin, hit.Position, Color.green, 0.1f);
                    
                    // �浹 ������ ���� ���� ǥ��
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
                // ���� ����� �������� ���� ���� ǥ�� (�Ķ���)
                Debug.DrawLine(origin, nearestHitPos, Color.blue, 0.2f);
                
                // ĳ���� ��ġ�� ���� ǥ�� (����Ÿ��)
                Debug.DrawLine(origin + new float3(0.2f, 0, 0), origin - new float3(0.2f, 0, 0), Color.magenta, 0.2f);
                Debug.DrawLine(origin + new float3(0, 0.2f, 0), origin - new float3(0, 0.2f, 0), Color.magenta, 0.2f);
                Debug.DrawLine(origin + new float3(0, 0, 0.2f), origin - new float3(0, 0, 0.2f), Color.magenta, 0.2f);
                
                Debug.Log($"[{entity.Index}] ���� ����� �� ��ġ: {nearestHitPos}, �Ÿ�: {minDistance}, �浹 ��ƼƼ: {hitEntity.Index}");
                // �ʿ��� �߰� ó�� ��ġ
            }
        }
    }
}