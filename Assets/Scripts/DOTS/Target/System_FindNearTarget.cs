using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

using static ConfigAuthoring;

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
        int numUnit= SystemAPI.QueryBuilder()
            .WithAll<UnitData>()
            .Build()
            .CalculateEntityCount();

        NativeArray<LocalTransform> TargetTransform
            = CollectionHelper.CreateNativeArray<LocalTransform>(numUnit, Allocator.TempJob);
        NativeArray<Entity> TargetEntity
            = CollectionHelper.CreateNativeArray<Entity>(numUnit, Allocator.TempJob);
        NativeArray<CAMP_TYPE> TargetCamp
            = CollectionHelper.CreateNativeArray<CAMP_TYPE>(numUnit, Allocator.TempJob);

        var job_collect = new Job_UnitCollect
        {
            TargetTransform = TargetTransform,
            TargetEntity    = TargetEntity,
            TargetCamp      = TargetCamp,
        };

        var handle_collect = job_collect.ScheduleParallel(state.Dependency);

        var job_find = new Job_FindNearTarget
        {
            TargetTransform = TargetTransform,
            TargetEntity    = TargetEntity,
            TargetCamp      = TargetCamp,
        };

        var handle_find = job_find.ScheduleParallel(handle_collect);
        state.Dependency = handle_find;
        handle_find.Complete();
    
        TargetTransform.Dispose();
        TargetEntity.Dispose();
        TargetCamp.Dispose();
    }
}
