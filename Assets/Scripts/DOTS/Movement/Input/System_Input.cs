using UnityEngine;

using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

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
        /*float3 moveDir = new float3
        (
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        moveDir = math.normalizesafe(moveDir);
        if (math.all(moveDir == float3.zero))
            return;
        */

        if (!SystemAPI.TryGetSingleton<JoystickInputData>(out var inputData))
            return;

         float dt = SystemAPI.Time.DeltaTime;

        foreach (var
            (
                transform,
                movement,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRW<MovementData>
            >()
            .WithEntityAccess())
        {
            float3 newPos
                = transform.ValueRO.Position
                + (inputData.dir.ToFloat3_XZ() * movement.ValueRO.moveSpeed * dt);

            if (inputData.jump)
            {
                newPos.y += 1f;
            }

            movement.ValueRW.hasNewPosition   = true;
            movement.ValueRW.moveNextPosition = newPos;
        }
    }
}
