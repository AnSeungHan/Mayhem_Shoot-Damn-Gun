using Unity.Entities;

[System.Serializable]
public enum CAMP_TYPE
{
    ALLIANCE,
    ENUMY
}

public struct UnitData
    : IComponentData
{
    public CAMP_TYPE    campType;
}