using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

using static MathematicsExtensions;

[BurstCompile]
public partial struct System_Ground : ISystem
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
        state.RequireForUpdate<PhysicsVelocity>();
        state.RequireForUpdate<MovementData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorldSystem = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var world = physicsWorldSystem.PhysicsWorld;

        foreach (var
            (
                transform,
                movement,
                velocity,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRW<MovementData>,
                RefRW<PhysicsVelocity>
            >()
            .WithEntityAccess())
        {
            float3 origin       = transform.ValueRO.Position;
            float3 end          = origin + (math.down() * 1000f);

            RaycastInput input = new RaycastInput
            {
                Start   = origin,
                End     = end,
                Filter  = filter
            };

            if (world.CastRay(input, out Unity.Physics.RaycastHit hit))
            {
                float dist = math.distance(origin, hit.Position);
                movement.ValueRW.isGround = (0.2f >= dist);

                DebugUtill.DrawTargetLine(origin, hit.Position, hit.SurfaceNormal, HexColor.green);
            }
            else
            {
                movement.ValueRW.isGround = false;
            }
        }
    }
}

