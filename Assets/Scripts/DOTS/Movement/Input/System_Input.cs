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
                slider,

                movement,
                velocity,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<InputData>,
                RefRO<LocalTransform>,
                RefRO<SlidWallData>,

                RefRW<MovementData>, 
                RefRW<PhysicsVelocity>
            >()
            .WithEntityAccess())
        {
            if (inputData.jump)
            {
                velocity.ValueRW.Linear.y = 8f;

                //movement.ValueRW.hasNewPosition = false;
            }

            if (slider.ValueRO.sliding &&
                !movement.ValueRO.isGround)
            {
                float speed = movement.ValueRO.moveSpeed * 2.5f;

                /*float3 newPos
                    = transform.ValueRO.Position
                    + (slider.ValueRO.dir * speed);

                movement.ValueRW.hasNewPosition     = true;
                movement.ValueRW.moveNextPosition   = newPos;*/

                float3 dir = slider.ValueRO.dir * speed;
                velocity.ValueRW.Linear.x = dir.x;
                velocity.ValueRW.Linear.z = dir.z;

                continue;
            }

            if (!inputData.dir.IsZero())
            {
                /*if (slider.ValueRO.sliding)
                {
                    float distToWall = math.distance
                    (
                        transform.ValueRO.Position,
                        slider.ValueRO.nearPosition
                    );

                    if (2f >= distToWall)
                    {
                        continue;
                    }
                }*/
                    
                float speed = (movement.ValueRO.isGround) 
                    ? (movement.ValueRO.moveSpeed) 
                    : (movement.ValueRO.moveSpeed * 0.65f);

                /*float3 newPos
                    = transform.ValueRO.Position
                    + (inputData.dir.ToFloat3_XZ() * speed * dt);*/

                float3 dir = inputData.dir.ToFloat3_XZ() * speed;

                velocity.ValueRW.Linear.x = dir.x;
                velocity.ValueRW.Linear.z = dir.z;

                /*movement.ValueRW.hasNewPosition   = true;
                movement.ValueRW.moveNextPosition = newPos;*/
            }
            else 
            {
                velocity.ValueRW.Linear.x = 0f;
                velocity.ValueRW.Linear.z = 0f;
            }
        }
    }
}
