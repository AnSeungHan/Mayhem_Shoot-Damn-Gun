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

    [Header("\n[ Unit Setting ]")]
    public CAMP_TYPE        campType;
    public MOVEMENT_MODE    movementMode;
    public CLUSTER_MODE     clusterMode;


    class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitData
            {
                campType        = authoring.campType
            });
            AddComponent(entity, new TargetData());
            AddComponent(entity, new MovementData
            {
                moveSpeed       = authoring.moveSpeed,
                acceleration    = authoring.acceleration,
                angularSpeed    = authoring.angularSpeed,
                stopDistance    = authoring.stopDistance,
            });

            Add_MovementMode(ref entity, authoring.movementMode);
            Add_CampeType(ref entity, authoring.campType);
        }

        private void Add_MovementMode(ref Entity entity, MOVEMENT_MODE mode)
        {
            switch (mode)
            {
                case MOVEMENT_MODE.DIRECTION:
                    AddComponent(entity, new MovementDirectionData());
                    break;

                case MOVEMENT_MODE.NAV_AGENT:
                    AddComponent(entity, new NavAgentData());
                    break;
            }
        }

        private void Add_CampeType(ref Entity entity, CAMP_TYPE type)
        {
            switch (type)
            {
                case CAMP_TYPE.ALLIANCE:
                    AddComponent(entity, new AllianceData());
                    break;

                case CAMP_TYPE.ENUMY:
                    AddComponent(entity, new EnumyData());
                    break;
            }
        }
    }
}

