using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Extensions;

using static MathematicsExtensions;

partial struct System_RaycastingWall : ISystem
{
    private static readonly CollisionFilter filter = new CollisionFilter
    {
        BelongsTo       = ~0u,
        CollidesWith    = (1 << 6),
        GroupIndex      = 0
    };

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld   = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

        var collector = new ClosestHitCollector<RaycastHit>(1f);

        foreach 
        (
            var 
            (
                transform,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>
            >()
            .WithAll<InputData>()
            .WithEntityAccess()
        )
        {
            float distance          = 10f;
            float closestDistance   = float.MaxValue;
            float3 start            = transform.ValueRO.Position + Float3.up;

            Entity closestHitEntity   = Entity.Null;
            float3 closestHitPosition = float3.zero;
            foreach (float3 direction in Float3.directions)
            {
                float3 end = start + direction * distance;

                var rayInput = new RaycastInput
                {
                    Start   = start,
                    End     = end,
                    Filter  = filter
                };
                
                if (collisionWorld.CastRay(rayInput, ref collector))
                {
                    var hit = collector.ClosestHit;
                    float hitDist = math.distance(start, hit.Position);

                    if (hitDist < closestDistance)
                    {
                        closestDistance    = hitDist;
                        closestHitEntity   = hit.Entity;
                        closestHitPosition = hit.Position;
                    }

                    DebugUtill.DrawLine(start, hit.Position, HexColor.green);
                }
            }

            if (Entity.Null == closestHitEntity)
                continue;

            DebugUtill.DrawLine(start, closestHitPosition, HexColor.red);
            DebugUtill.DrawPoint(closestHitPosition, HexColor.green);
        }
    }

    /*public void OnUpdate(ref SystemState state)
    {
        var physicsWorld   = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

        var collector = new ClosestHitCollector<RaycastHit>(1f);
        var filter = new CollisionFilter
        {
            BelongsTo       = ~0u,
            CollidesWith    = ~0u,
            GroupIndex      = 0
        };

        foreach 
        (
            var 
            (
                transform,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>
            >()
            .WithAll<InputData>()
            .WithEntityAccess()
        )
        {
            NativeList<ColliderCastHit> outHits = new NativeList<ColliderCastHit>(Allocator.Temp);

            if (collisionWorld.SphereCastAll
                (
                    transform.ValueRO.Position,
                    1f,
                    Float3.forward,
                    1f,
                    ref outHits,
                    filter
                ))
            {
                if (0 == outHits.Length)
                    continue;

                var closest = outHits[0];
                for (int i = 1; i < outHits.Length; i++)
                {
                    if (outHits[i].Fraction < closest.Fraction)
                        closest = outHits[i];
                }

                Entity hitEntity = closest.Entity;
                float3 hitPos = closest.Position;

                DebugUtill.DrawLine(transform.ValueRO.Position, hitPos, HexColor.red);
                DebugUtill.DrawPoint(hitPos, HexColor.green);
            }

            outHits.Dispose();
        }
    }*/
}