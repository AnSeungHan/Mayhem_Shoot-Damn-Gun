using Unity.Entities;
using Unity.Transforms;

public struct TargetData
    : IComponentData
{
    public Entity           targetEntity;
    public LocalTransform   targetTransform;
}

public struct SearchAngleData
    : IComponentData
{
    public float            searchRange;
    public float            findAngle_begin;
    public float            findAngle_end;
}