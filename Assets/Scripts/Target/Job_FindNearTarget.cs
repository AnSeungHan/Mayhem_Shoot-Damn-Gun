using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public partial struct Job_FindNearTarget_Alliance : IJobEntity
{
    [ReadOnly]
    public NativeArray<LocalTransform> TargetTransform;
    [ReadOnly]
    public NativeArray<Entity> TargetEntity;

    [BurstCompile]
    public void Execute(in AllianceData alliance, in LocalTransform transform, ref TargetData target)
    {
        float minDistSq = float.MaxValue;
        Entity closestEntity = Entity.Null;
        LocalTransform closestTransform = default;

        float3 myPos = transform.Position;

        for (int i = 0; i < TargetTransform.Length; ++i) 
        {
            float3 targetPos = TargetTransform[i].Position;

            float dist = math.distance(myPos, targetPos);
            if (dist < minDistSq)
            {
                minDistSq = dist;

                closestEntity = TargetEntity[i];
                closestTransform = TargetTransform[i];
            }
        }

        if (closestEntity != Entity.Null)
        {
            target.targetEntity = closestEntity;
            target.targetTransform = closestTransform;
        }
    }
}

public partial struct Job_FindNearTarget_Enumy : IJobEntity
{
    [ReadOnly]
    public NativeArray<LocalTransform> TargetTransform;
    [ReadOnly]
    public NativeArray<Entity> TargetEntity;

    [BurstCompile]
    public void Execute(in EnumyData alliance, in LocalTransform transform, ref TargetData target)
    {
        float minDistSq = float.MaxValue;
        Entity closestEntity = Entity.Null;
        LocalTransform closestTransform = default;

        float3 myPos = transform.Position;

        for (int i = 0; i < TargetTransform.Length; ++i)
        {
            float3 targetPos = TargetTransform[i].Position;

            float dist = math.distance(myPos, targetPos);
            if (dist < minDistSq)
            {
                minDistSq = dist;

                closestEntity = TargetEntity[i];
                closestTransform = TargetTransform[i];
            }
        }

        if (closestEntity != Entity.Null)
        {
            target.targetEntity = closestEntity;
            target.targetTransform = closestTransform;
        }
    }
}