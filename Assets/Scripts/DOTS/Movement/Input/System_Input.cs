using UnityEngine;

using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

using static MathematicsExtensions;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct System_Input : ISystem
{
    /*[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputData>();
    }*/

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<JoystickInputData>(out var inputData))
            return;

         float dt = SystemAPI.Time.DeltaTime;

        foreach (var
            (
                input,
                transform,
                movement,
                velocity,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<InputData>,
                RefRO<LocalTransform>,
                RefRW<MovementData>,
                RefRW<PhysicsVelocity>
            >()
            .WithEntityAccess())
        {
            if (inputData.jump)
            {
                velocity.ValueRW.Linear.y = 8f;

                movement.ValueRW.hasNewPosition = false;
            }

            if (!inputData.dir.IsZero())
            {
                float speed = movement.ValueRO.moveSpeed;

                float3 newPos
                    = transform.ValueRO.Position
                    + (inputData.dir.ToFloat3_XZ() * speed * dt);

                movement.ValueRW.hasNewPosition   = true;
                movement.ValueRW.moveNextPosition = newPos;
            }
        }
    }
}
