using UnityEngine;

using Unity.Entities;

using static ConfigAuthoring;

public class UnitAuthoring : MonoBehaviour
{
    [Header("\n[ Info ]")]
    public float            moveSpeed    = 1f;
    public float            acceleration = 1f;
    public float            angularSpeed = 1f;
    public float            stopDistance = 1f;

    class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitData
            {
                campType        = CAMP_TYPE.ENUMY
            });
            AddComponent(entity, new MovementData
            {
                moveSpeed       = authoring.moveSpeed,
                acceleration    = authoring.acceleration,
                angularSpeed    = authoring.angularSpeed,
                stopDistance    = authoring.stopDistance,
            });
            AddComponent(entity, new TargetData());
            //AddComponent(entity, new NavAgentData());
            AddComponent(entity, new MovementDirectionData());
            //AddComponent(entity, new Cluster_BOIDSData());
        }
    }
}

