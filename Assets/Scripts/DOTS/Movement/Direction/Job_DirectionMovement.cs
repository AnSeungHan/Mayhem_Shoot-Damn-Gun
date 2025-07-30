using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

using static MathematicsExtensions;

[BurstCompile]
public partial struct Job_DirectionMovement : IJobEntity
{
    public float deltaTime;

    public void Execute
    (
        ref MovementData            movement,
        ref LocalTransform          transform,
        ref PhysicsVelocity         velocity,

        in  MovementDirectionData   moveDir,
        in  TargetData              target
    )
    {
        if (Entity.Null == target.targetEntity)
        {
            velocity.Linear.x = 0f;
            velocity.Linear.z = 0f;

            return;
        }

        float3 direction
            = target.targetTransform.Position
            - transform.Position;
        float speed = movement.moveSpeed;

        /*if (math.distance(transform.Position, moveData.moveNextPosition) < 0.01f)
        {
            moveData.hasNewPosition = false;

            return;
        }*/

        movement.hasNewPosition = true;
        movement.moveNextPosition 
            = transform.Position 
            + (direction * speed * deltaTime);

        float3 dir
            = movement.moveNextPosition.normalize()
            * -movement.moveSpeed;

        velocity.Linear.x = dir.x;
        velocity.Linear.z = dir.z;
    }
}
