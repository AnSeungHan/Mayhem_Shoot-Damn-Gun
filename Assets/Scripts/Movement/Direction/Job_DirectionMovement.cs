using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct Job_DirectionMovement : IJobEntity
{
    public float deltaTime;

    public void Execute
    (
        in  MovementData            moveData,
        in  MovementDirectionData   movement,
        in  TargetData              target,
        ref LocalTransform          transform
    )
    {
        if (Entity.Null == target.targetEntity)
            return;

        float3 direction
            = target.targetTransform.Position
            - transform.Position;
        float speed = moveData.moveSpeed;

        transform.Position += direction * speed * deltaTime;
    }
}
