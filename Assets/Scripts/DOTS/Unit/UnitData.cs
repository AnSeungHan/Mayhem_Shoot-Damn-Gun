using Unity.Collections;
using Unity.Entities;

[System.Serializable]
public enum CAMP_TYPE
{
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

    public int          stat_health;    // 체력
    public int          stat_shield;    // 쉴드
    public int          stat_defensive; // 장갑

    public NativeArray<DamageValue> atk;     // 일반 데미지
}

//bool hasComponent = SystemAPI.EntityManager.HasComponent<YourComponent>(entity);

public struct AllianceData
    : IComponentData
{ }

public struct EnumyData
    : IComponentData
{ }