using UnityEngine;

using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct System_Input : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 moveDir = new float3
        (
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        moveDir = math.normalizesafe(moveDir);
        if (math.all(moveDir == float3.zero))
            return;

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var
            (
                input,
                transform,
                movement,
                entity
            )
            in SystemAPI.Query
            <
                RefRO<InputData>,
                RefRO<LocalTransform>,
                RefRW<MovementData>
            >()
            .WithEntityAccess())
        {
            float3 newPos
                = transform.ValueRO.Position
                + (moveDir * movement.ValueRO.moveSpeed * dt);

            movement.ValueRW.hasNewPosition = true;
            movement.ValueRW.moveNextPosition = newPos;
        }
    }
}
