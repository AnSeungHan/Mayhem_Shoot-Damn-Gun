using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct Job_Movement : IJobEntity
{
    public void Execute
    (
        in  MovementData            moveData,
        ref LocalTransform          transform
    )
    {
       if (!moveData.hasNewPosition)
            return;

       transform.Position = moveData.moveNextPosition;
    }
}
