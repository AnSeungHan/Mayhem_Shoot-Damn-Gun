using Unity.Entities;
using Unity.Transforms;

public struct TargetData
    : IComponentData
{
    public Entity           targetEntity;
    public LocalTransform   targetTransform;
}
