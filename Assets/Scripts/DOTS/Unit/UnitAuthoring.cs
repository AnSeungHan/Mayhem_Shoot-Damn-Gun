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
    public bool             isMovement;

    [Header("\n[ Owenr Setting ]")]
    public GameObject       owner;

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

            if (authoring.isMovement)
            { 
                AddComponent(entity, new MovementData
                {
                    moveSpeed       = authoring.moveSpeed,
                    acceleration    = authoring.acceleration,
                    angularSpeed    = authoring.angularSpeed,
                    stopDistance    = authoring.stopDistance,
                });
            }

            Add_MovementMode(ref entity, authoring);
            Add_ClusterModeType(ref entity, authoring);
            Add_CampeType(ref entity, authoring);
        }

        private void Add_MovementMode(ref Entity entity, UnitAuthoring authoring)
        {
            switch (authoring.movementMode)
            {
                case MOVEMENT_MODE.DIRECTION:
                    AddComponent(entity, new MovementDirectionData());
                    break;

                case MOVEMENT_MODE.OWNER_ROTATION:
                    AddComponent(entity, new MovementOwnerRotationData() 
                    {
                        owner   = GetEntity(authoring.owner, TransformUsageFlags.None),
                        offset  = 1f
                    });
                    break;

                case MOVEMENT_MODE.NAV_AGENT:
                    AddComponent(entity, new NavAgentData(){});
                    break;

                case MOVEMENT_MODE.INPUT:
                    AddComponent(entity, new InputData());
                    break;
            }
        }

        private void Add_ClusterModeType(ref Entity entity, UnitAuthoring authoring)
        {
            switch (authoring.clusterMode)
            {
                case CLUSTER_MODE.BOIDS:
                    AddComponent(entity, new Cluster_BOIDSData());
                    break;
            }
        }

        private void Add_CampeType(ref Entity entity, UnitAuthoring authoring)
        {
            switch (authoring.campType)
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

