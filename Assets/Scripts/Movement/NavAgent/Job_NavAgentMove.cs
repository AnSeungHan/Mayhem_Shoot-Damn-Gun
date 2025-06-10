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
            !agent.isValid ||
            !agent.pathBlob.IsCreated)
        {
            return;
        }

        var currentPosition = transform.Position;

        float separationRadius = 1.5f;  // �ʹ� ����� ���� �о�� �Ÿ�
        float3 separation = float3.zero;

        for (int i = 0; i < AllTransforms.Length; i++)
        {
            var otherTransform = AllTransforms[i];
            var otherPos = otherTransform.Position;


            if (entityIndex == i)
                continue;

            // Y�� �����ؼ� 2D �Ÿ� ���
            float3 pos2D    = new float3(otherPos.x, 0, otherPos.z);
            float3 ownPos2D = new float3(currentPosition.x, 0, currentPosition.z);

            float dist = math.distance(pos2D, ownPos2D);

            // �и���: �ʹ� ������ �ݴ� �������� �о
            if (dist < separationRadius &&
                dist > 0)
            {
                separation
                    += (ownPos2D - pos2D)
                    / (dist * dist);
            }
        }

        float separationWeight = 1.5f;
        float3 desiredDirection = separation * separationWeight;

        // ��ǥ ��� ���� ���ϱ�
        ref var pathBlob = ref agent.pathBlob.Value;
        var currentTarget = (agent.currentCornerIndex < pathBlob.Corners.Length)
            ? (pathBlob.Corners[agent.currentCornerIndex])
            : (float3.zero);

        float3 targetDirection = math.normalize(currentTarget - currentPosition);

        // �и��� + ��ǥ ���� �ռ�
        float3 finalDirection = math.normalize(desiredDirection + targetDirection);

        // Y�� ����
        float3 moveDir = new float3(finalDirection.x, 0, finalDirection.z);

        if (math.lengthsq(moveDir) < 1f)
        {
            return;
        }

        // ȸ�� ó��
        var targetRotation = quaternion.LookRotationSafe(moveDir, math.up());
        transform.Rotation = math.slerp
        (
            transform.Rotation,
            targetRotation,
            movement.angularSpeed * DeltaTime
        );

        // �̵� ó��
        var targetSpeed = movement.moveSpeed;
        movement.curSpeed = math.lerp
        (
            movement.curSpeed,
            targetSpeed,
            movement.acceleration * DeltaTime
        );

        var moveDistance
            = movement.curSpeed
            * DeltaTime;

        float3 moveDelta
            = math.normalize(moveDir)
            * math.min(moveDistance, math.distance(currentPosition, currentTarget));

        if (HasNaN(moveDelta))
            return;

        float3 nextMove
            = transform.Position
            + moveDelta;

        transform.Position = nextMove;
    }

    public static bool HasNaN(float3 v)
    {
        return math.isnan(v.x) ||
                math.isnan(v.y) ||
                math.isnan(v.z);
    }
}
