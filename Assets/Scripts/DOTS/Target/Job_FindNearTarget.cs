using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public partial struct Job_FindNearTarget : IJobEntity
{
    [ReadOnly]
    public NativeArray<LocalTransform> TargetTransform;
    [ReadOnly]
    public NativeArray<Entity> TargetEntity;
    [ReadOnly]
    public NativeArray<CAMP_TYPE> TargetCamp;

    [BurstCompile]
    public void Execute
    (
        [EntityIndexInQuery]
        int                     entityIndex,

        in  UnitData            unit, 
        in  LocalTransform      transform, 
        ref TargetData          target
    )
    {
        float minDistSq = float.MaxValue;
        Entity closestEntity = Entity.Null;
        LocalTransform closestTransform = default;

        float3 myPos = transform.Position;

        for (int i = 0; i < TargetTransform.Length; ++i) 
        {
            if (CAMP_TYPE.NONE == TargetCamp[i])
                continue;

            if (entityIndex == i)
                continue;

            if (unit.campType == TargetCamp[i])
                continue;

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
            target.targetEntity    = closestEntity;
            target.targetTransform = closestTransform;
        }
        else
        {
            target.targetEntity    = Entity.Null;
        }
    }
}

public partial struct Job_UnitCollect : IJobEntity
{
    public NativeArray<LocalTransform>  TargetTransform;
    public NativeArray<Entity>          TargetEntity;
    public NativeArray<CAMP_TYPE>       TargetCamp;

    [BurstCompile]
    public void Execute
    (
        [EntityIndexInQuery]
        int                     entityIndex,

        in  Entity              entity,
        in  UnitData            unit,
        in  LocalTransform      transform
    )
    {
        TargetTransform[entityIndex] = transform;
        TargetEntity[entityIndex]    = entity;
        TargetCamp[entityIndex]      = unit.campType;

        if (CAMP_TYPE.NONE == TargetCamp[entityIndex])
            return;
    }
}