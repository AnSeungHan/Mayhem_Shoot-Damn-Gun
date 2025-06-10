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
    }

}

