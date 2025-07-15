using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct Job_Movement : IJobEntity
{
    public float deltaTime;

    private const float jumpDuration = 1f; // �ö󰡴� �ð�
    private const float jumpHeight   = 5f; // �ִ� ����

    public void Execute
    (
        ref MovementData            moveData,
        ref LocalTransform          transform
    )
    {
        if (!moveData.hasNewPosition)
            return;

        transform.Position = new float3
        (
             moveData.moveNextPosition.x,
             transform.Position.y,
             moveData.moveNextPosition.z
        );

        moveData.hasNewPosition = false;
    }
}
