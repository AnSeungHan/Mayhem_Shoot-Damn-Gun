using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct Job_NavAgentMove : IJobEntity
{
    [ReadOnly]
    public float DeltaTime;
    [ReadOnly]
    public NativeArray<LocalToWorld> AllTransforms;

    [BurstCompile]
    public void Execute
    (
        [EntityIndexInQuery]
        int                 entityIndex,
        in  Entity          entity,

        ref MovementData    movement,
        ref NavAgentData    agent,
        ref LocalTransform  transform
    )
    {
        if (!agent.isHasPath ||
            !agent.pathBlob.IsCreated)
        {
            return;
        }

        ref var pathBlob = ref agent.pathBlob.Value;
        if (agent.currentCornerIndex >= pathBlob.Corners.Length)
        {
            agent.isReachedDestination = true;
            agent.isHasPath            = false;
            movement.curSpeed          = 0f;

            return;
        }

        var currentTarget = pathBlob.Corners[agent.currentCornerIndex];
        var currentPos = transform.Position;
        var direction = currentTarget - currentPos;
        var distance = math.length(direction);

        // ���� ��ǥ���� �����ߴ��� Ȯ��
        if (distance <= movement.stopDistance)
        {
            ++agent.currentCornerIndex;

            if (agent.currentCornerIndex >= pathBlob.Corners.Length)
            {
                agent.isReachedDestination = true;
                agent.isHasPath            = false;
                movement.curSpeed          = 0f;

                return;
            }

            return;
        }

        // �̵� ���� ���
        direction = math.normalize(direction);

        // �ӵ� ��� (���ӵ� ����)
        var targetSpeed = movement.moveSpeed;
        movement.curSpeed = math.lerp
        (
            movement.curSpeed,
            targetSpeed,
            movement.acceleration * DeltaTime
        );

        // ȸ�� ���
        if (math.lengthsq(direction) > 0.001f)
        {
            var targetRotation = quaternion.LookRotationSafe(direction, math.up());
            transform.Rotation = math.slerp
            (
                transform.Rotation,
                targetRotation,
                movement.acceleration * DeltaTime
            );
        }

        // �̵� ���
        var moveDistance
            = movement.moveSpeed
            * DeltaTime;

        transform.Position
            += direction
            * math.min(moveDistance, distance);
    }

    public static bool HasNaN(float3 v)
    {
        return math.isnan(v.x) ||
                math.isnan(v.y) ||
                math.isnan(v.z);
    }
}
