using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static ConfigAuthoring;

[BurstCompile]
[UpdateAfter(typeof(System_Input))]
partial struct System_OwnerRotation : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config_Info>();
        state.RequireForUpdate<MovementOwnerRotationData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: false);

        foreach (var 
            (
                ownerRotation,
                target,
                transform,
                enitity
            )
            in SystemAPI.Query
            <
                RefRO<MovementOwnerRotationData>,
                RefRO<TargetData>,
                RefRW<LocalTransform>
            >()
            .WithEntityAccess())
        {
            var ownerEntity = ownerRotation.ValueRO.owner;
            var offset = ownerRotation.ValueRO.offset;

            var owner = ownerRotation.ValueRO.owner;
            if (owner == Entity.Null ||
                !transformLookup.HasComponent(owner))
                continue;

            var ownerTransform = transformLookup[ownerEntity];
            var targetPos = target.ValueRO.targetTransform.Position;
            var ownerPos = ownerTransform.Position;

            // 1. 방향 계산
            targetPos.y = ownerPos.y = 0;
            float3 direction = math.normalizesafe(targetPos - ownerPos);
            if (math.all(direction == float3.zero)) 
                continue;

            // 2. 위치 이동 (owner 중심에서 offset만큼 떨어지게)
            float3 newPos = (direction * offset);
            //float3 newPos = targetPos;
            newPos.y = transform.ValueRW.Position.y;

            transform.ValueRW.Position = newPos;

            /*// 3. 해당 방향을 바라보는 회전 적용
            quaternion newRot = quaternion.LookRotationSafe(direction, math.up());
            transform.ValueRW.Rotation = newRot;*/
        }
    }
}
