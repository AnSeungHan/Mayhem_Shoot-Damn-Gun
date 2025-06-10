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

        float separationRadius = 1.5f;  // 너무 가까운 유닛 밀어내는 거리
        float3 separation = float3.zero;

        for (int i = 0; i < AllTransforms.Length; i++)
        {
            var otherTransform = AllTransforms[i];
            var otherPos = otherTransform.Position;


            if (entityIndex == i)
                continue;

            // Y축 고정해서 2D 거리 계산
            float3 pos2D    = new float3(otherPos.x, 0, otherPos.z);
            float3 ownPos2D = new float3(currentPosition.x, 0, currentPosition.z);

            float dist = math.distance(pos2D, ownPos2D);

            // 분리력: 너무 가까우면 반대 방향으로 밀어냄
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

        // 목표 경로 방향 구하기
        ref var pathBlob = ref agent.pathBlob.Value;
        var currentTarget = (agent.currentCornerIndex < pathBlob.Corners.Length)
            ? (pathBlob.Corners[agent.currentCornerIndex])
            : (float3.zero);

        float3 targetDirection = math.normalize(currentTarget - currentPosition);

        // 분리력 + 목표 방향 합성
        float3 finalDirection = math.normalize(desiredDirection + targetDirection);

        // Y축 고정
        float3 moveDir = new float3(finalDirection.x, 0, finalDirection.z);

        if (math.lengthsq(moveDir) < 1f)
        {
            return;
        }

        // 회전 처리
        var targetRotation = quaternion.LookRotationSafe(moveDir, math.up());
        transform.Rotation = math.slerp
        (
            transform.Rotation,
            targetRotation,
            movement.angularSpeed * DeltaTime
        );

        // 이동 처리
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
