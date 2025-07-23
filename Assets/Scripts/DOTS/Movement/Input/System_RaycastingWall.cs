using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;

using static MathematicsExtensions;

partial struct System_RaycastingWall : ISystem
{
    private const           float           distance    = 1.5f;
    private static readonly CollisionFilter filter      = new CollisionFilter
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
                movement,
                slider,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRO<MovementData>,

                RefRW<SlidWallData>
            >()
            .WithAll<InputData>()
            .WithEntityAccess()
        )
        {
            float closestDistance   = float.MaxValue;
            float3 start            = transform.ValueRO.Position + Float3.up;

            Entity      closestEntity   = Entity.Null;
            RaycastHit  closestHit      = default;
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
                        closestDistance     = hitDist;
                        closestEntity       = hit.Entity;
                        closestHit          = hit;
                    }

                    DebugUtill.DrawTargetLine(start, hit.Position, hit.SurfaceNormal, HexColor.green);
                }
            }

            if (Entity.Null == closestEntity)
            {
                slider.ValueRW.sliding = false;
            }
            else
            {
                float3 normal   = math.normalize(closestHit.SurfaceNormal);
                float3 slideDir = math.normalize(math.cross(normal, math.up()));

                slider.ValueRW.nearPosition = closestHit.Position;
                slider.ValueRW.dir          = slideDir;
                slider.ValueRW.sliding      = true;

                DebugUtill.DrawTargetLine(start, closestHit.Position, closestHit.SurfaceNormal, HexColor.red);
            }
        }
    }
}