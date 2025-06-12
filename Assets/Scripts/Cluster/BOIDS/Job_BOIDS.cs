using UnityEngine;
using UnityEngine.AI;

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

    [ReadOnly]
    public NativeArray<float3> NavMeshVertices;
    [ReadOnly] 
    public NativeArray<int> NavMeshIndices;


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
        float3 finalDirection   = math.normalize(desiredDirection + targetDirection);  // 분리력 + 목표 방향 합성
        float3 moveDir          = new float3(finalDirection.x, 0, finalDirection.z);   // Y축 고정

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

        if (!IsInsideNavMesh(nxtPos))
        {
            movement.hasNewPosition = false;
            movement.moveNextPosition = transform.Position;

            return;
        }

        movement.hasNewPosition = true;
        movement.moveNextPosition = nextMove;
    }

    public static bool HasNaN(float3 v)
    {
        return math.isnan(v.x) ||
               math.isnan(v.y) ||
               math.isnan(v.z);
    }

    bool IsInsideNavMesh(float3 point)
    {
        float2 p = new float2(point.x, point.z);

        for (int i = 0; i < NavMeshIndices.Length; i += 3)
        {
            float3 a = NavMeshVertices[NavMeshIndices[i]];
            float3 b = NavMeshVertices[NavMeshIndices[i + 1]];
            float3 c = NavMeshVertices[NavMeshIndices[i + 2]];

            float2 a2D = new float2(a.x, a.z);
            float2 b2D = new float2(b.x, b.z);
            float2 c2D = new float2(c.x, c.z);

            if (IsPointInTriangle(p, a2D, b2D, c2D))
                return true;
        }

        return false;
    }

    bool IsPointInTriangle(float2 p, float2 a, float2 b, float2 c)
    {
        float2 v0 = c - a;
        float2 v1 = b - a;
        float2 v2 = p - a;

        float dot00 = math.dot(v0, v0);
        float dot01 = math.dot(v0, v1);
        float dot02 = math.dot(v0, v2);
        float dot11 = math.dot(v1, v1);
        float dot12 = math.dot(v1, v2);

        float denom = dot00 * dot11 - dot01 * dot01;
        if (denom == 0) return false;

        float u = (dot11 * dot02 - dot01 * dot12) / denom;
        float v = (dot00 * dot12 - dot01 * dot02) / denom;

        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }
}
