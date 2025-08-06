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
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<JoystickInputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<JoystickInputData>(out var inputData))
            return;

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var
            (
                transform,
                slider,

                input,
                movement,
                velocity,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRO<SlidWallData>,

                RefRW<InputData>,
                RefRW<MovementData>, 
                RefRW<PhysicsVelocity>
            >()
            .WithEntityAccess())
        {
            if (!inputData.dir.IsZero())
            {
                input.ValueRW.preDir = input.ValueRO.curDir;
                input.ValueRW.curDir = inputData.dir;
            }

            if (inputData.jump)
            {
                velocity.ValueRW.Linear.y = 8f;

                //movement.ValueRW.hasNewPosition = false;
            }

            if (slider.ValueRO.sliding &&
                !movement.ValueRO.isGround)
            {
                float speed = movement.ValueRO.moveSpeed * 1.85f;

                float2 inputDir     = (!inputData.dir.IsZero() || input.ValueRO.preDir.IsZero()) 
                    ? (inputData.dir.normalize())
                    : (input.ValueRO.preDir.normalize());
                float2 sliderDir    = slider.ValueRO.dir.xz.normalize();
                float  dot          = math.dot(inputDir, sliderDir);

                float3 dir 
                    = slider.ValueRO.dir 
                    * ((dot >= 0f) ? (speed) : (-speed));

                velocity.ValueRW.Linear.x = dir.x;
                velocity.ValueRW.Linear.z = dir.z;

                continue;
            }

            if (!inputData.dir.IsZero())
            {
                float speed = (movement.ValueRO.isGround) 
                    ? (movement.ValueRO.moveSpeed) 
                    : (movement.ValueRO.moveSpeed * 0.65f);

                float3 dir = inputData.dir.ToFloat3_XZ() * speed;

                velocity.ValueRW.Linear.x = dir.x;
                velocity.ValueRW.Linear.z = dir.z;
            }
            else 
            {
                velocity.ValueRW.Linear.x = 0f;
                velocity.ValueRW.Linear.z = 0f;
            }
        }
    }
}
