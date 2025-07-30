using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

using static MathematicsExtensions;

[BurstCompile]
public partial struct Job_Movement : IJobEntity
{
    public float deltaTime;

    public void Execute
    (
        ref LocalTransform          transform,
        ref PhysicsVelocity         velocity,
        ref MovementData            movement,
        ref NavAgentData            navAgent
    )
    {
        if (!movement.hasNewPosition)
        {
            velocity.Linear.x = 0f;
            velocity.Linear.z = 0f;

            return;
        }

        float3 dir
            = movement.moveNextPosition.normalize()
            * -movement.moveSpeed;

        velocity.Linear.x = dir.x;
        velocity.Linear.z = dir.z;

        movement.hasNewPosition = false;
    }
}
