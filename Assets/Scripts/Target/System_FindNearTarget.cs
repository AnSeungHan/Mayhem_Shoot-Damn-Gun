using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

using static ConfigAuthoring;
using Unity.Jobs;

[BurstCompile]
public partial struct System_FindNearTarget : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<Config_FindTarget_LinearSearch>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // 아군이 적군 찾기
        int numTargets = SystemAPI.QueryBuilder()
            .WithAll<EnumyData>()
            .Build()
            .CalculateEntityCount();

        NativeArray<LocalTransform> TargetTransform = CollectionHelper.CreateNativeArray<LocalTransform>(numTargets, Allocator.TempJob);
        NativeArray<Entity>         TargetEntity    = CollectionHelper.CreateNativeArray<Entity>(numTargets, Allocator.TempJob);

        int idx = 0;
        foreach (var 
            (
                transform,
                enumy,

                entity
            ) 
            in SystemAPI.Query
            <
                RefRW<LocalTransform>,
                RefRO<EnumyData>
            >()
            .WithEntityAccess())
        {
            TargetTransform[idx] = transform.ValueRO;
            TargetEntity[idx] = entity;

            ++idx;
        }

        var job_alliance = new Job_FindNearTarget_Alliance
        {
            TargetTransform = TargetTransform,
            TargetEntity    = TargetEntity
        };

        JobHandle jobHandle = job_alliance.ScheduleParallel(state.Dependency);

        TargetTransform.Dispose(jobHandle);
        TargetEntity.Dispose(jobHandle);

        state.Dependency = jobHandle;

        // 적군이 아군 찾기
        /*numTargets = SystemAPI.QueryBuilder()
            .WithAll<AllianceData>()
            .Build()
            .CalculateEntityCount();

        TargetTransform = CollectionHelper.CreateNativeArray<LocalTransform>(numTargets, Allocator.TempJob);
        TargetEntity    = CollectionHelper.CreateNativeArray<Entity>(numTargets, Allocator.TempJob);

        idx = 0;
        foreach (var
            (
                transform,
                enumy,

                entity
            )
            in SystemAPI.Query
            <
                RefRO<LocalTransform>,
                RefRO<AllianceData>
            >()
            .WithEntityAccess())
        {
            TargetTransform[idx] = transform.ValueRO;
            TargetEntity[idx] = entity;

            ++idx;
        }

        var job_enumy = new Job_FindNearTarget_Enumy
        {
            TargetTransform = TargetTransform,
            TargetEntity    = TargetEntity
        };

        var handle_enumy = job_enumy.ScheduleParallel(state.Dependency);

        TargetTransform.Dispose(handle_enumy);
        TargetEntity.Dispose(handle_enumy);*/
    }
}
