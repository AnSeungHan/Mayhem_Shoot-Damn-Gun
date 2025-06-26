using Unity.Collections;
using Unity.Entities;

[System.Serializable]
public enum CAMP_TYPE
{
    NONE,
    ALLIANCE,
    ENUMY
}

public enum DMG_TYPE
{
    Normal,
    Heavy,
}

public struct DamageValue
{
    public DMG_TYPE damageType;
    public int      value;
}

public struct UnitData
    : IComponentData
{
    public CAMP_TYPE    campType;

    public int          stat_health;    // ü��
    public int          stat_shield;    // ����
    public int          stat_defensive; // �尩
}

//bool hasComponent = SystemAPI.EntityManager.HasComponent<YourComponent>(entity);

public struct AllianceData
    : IComponentData
{ }

public struct EnumyData
    : IComponentData
{ }