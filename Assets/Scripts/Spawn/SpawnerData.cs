using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerData : IComponentData
{
    public Entity   createPrefab;

    public int      numCreate;
    public float2   bounds;
}

public struct SpawnerData_Click : IComponentData
{
    public Entity   createPrefab;

    public bool     isOnStartCreate;
    public bool     isDynamicCreate;
    public int      numCreate;
    public float2   bounds;
}

