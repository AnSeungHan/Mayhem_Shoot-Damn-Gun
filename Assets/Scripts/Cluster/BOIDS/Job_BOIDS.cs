using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public partial struct Job_BOIDS : IJobEntity
{
    [ReadOnly]
    public float DeltaTime;
    [ReadOnly]
    public NativeArray<LocalToWorld> AllTransforms;

    const float separationRadius = 1.5f;  // 너무 가까운 유닛 밀어내는 거리
    const float separationWeight = 1.5f;

    public void Execute
    (
        [EntityIndexInQuery]
        int                 entityIndex,

        in  LocalTransform  transform,
        ref MovementData    movement
    )
    {
        if (!movement.hasNewPosition)
            return;

        float3 curPos = transform.Position;
        float3 nxtPos = movement.moveNextPosition; 
        float3 separation = float3.zero;

        for (int i = 0; i < AllTransforms.Length; i++)
        {
            var otherTransform = AllTransforms[i];
            var otherPos = otherTransform.Position;

            if (entityIndex == i)
                continue;

            // Y축 고정해서 2D 거리 계산
            float3 pos2D    = new float3(otherPos.x, 0, otherPos.z);
            float3 ownPos2D = new float3(curPos.x  , 0, curPos.z);

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

        float3 desiredDirection = separation * separationWeight;                      // 분리력
        float3 targetDirection  = math.normalize(nxtPos - curPos);                    // 목표 경로 방향 구하기
        float3 finalDirection  = math.normalize(desiredDirection + targetDirection);  // 분리력 + 목표 방향 합성
        float3 moveDir         = new float3(finalDirection.x, 0, finalDirection.z);   // Y축 고정

        if (math.lengthsq(moveDir) < 1f)
        {
            return;
        }

        float3 moveDelta
            = math.normalize(moveDir)
            * math.distance(curPos, nxtPos);

        if (HasNaN(moveDelta))
            return;

        float3 nextMove
            = transform.Position
            + moveDelta;

        movement.hasNewPosition = true;
        movement.moveNextPosition = nextMove;
    }

    public static bool HasNaN(float3 v)
    {
        return math.isnan(v.x) ||
                math.isnan(v.y) ||
                math.isnan(v.z);
    }
}
