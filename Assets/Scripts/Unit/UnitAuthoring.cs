using UnityEngine;

using Unity.Entities;

using static ConfigAuthoring;

public class UnitAuthoring : MonoBehaviour
{
    [Header("\n[ Info ]")]
    public float            moveSpeed;

    [Header("\n[ Unit Setting ]")]
    public CAMP_TYPE        campType;
    public MOVEMENT_MODE    movementMode;
    public CLUSTER_MODE     clusterMode;


    class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitData());
            AddComponent(entity, new TargetData());
            AddComponent(entity, new MovementData
            {
                moveSpeed = authoring.moveSpeed
            });

            Add_CampType(ref entity, authoring.campType);
            Add_MovementMode(ref entity, authoring.movementMode);
        }

        private void Add_CampType(ref Entity entity, CAMP_TYPE mode)
        {
            switch (mode)
            {
                case CAMP_TYPE.ALLIANCE:
                    AddComponent(entity, new AllianceData());
                    break;

                case CAMP_TYPE.ENUMY:
                    AddComponent(entity, new EnumyData());
                    break;
            }
        }

        private void Add_MovementMode(ref Entity entity, MOVEMENT_MODE mode)
        {
            switch (mode)
            {
                case MOVEMENT_MODE.DIRECTION:
                    AddComponent(entity, new MovementDirectionData());
                    break;

                case MOVEMENT_MODE.NAV_AGENT:
                    break;
            }
        }
    }

    [System.Serializable]
    public enum CAMP_TYPE
    {
        ALLIANCE,
        ENUMY
    }
}

