using UnityEngine;

using Unity.Entities;

public class ConfigAuthoring : MonoBehaviour
{
    [Header("\n[ Project Setting ]")]
    public MOVEMENT_MODE    movementMode;
    public CLUSTER_MODE     clusterMode;
    public FIND_TARGET_MODE findTargetMode;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);

            AddComponent(entity, new Config_Info
            {
                movementMode    = authoring.movementMode,
                clusterMode     = authoring.clusterMode,
                findTargetMode  = authoring.findTargetMode,
            });

            Add_MovementMode(ref entity, authoring.movementMode);
            Add_ClusterMode(ref entity, authoring.clusterMode);
            Add_FindTargetMode(ref entity, authoring.findTargetMode);
        }

        private void Add_MovementMode(ref Entity entity, MOVEMENT_MODE mode)
        {
            switch (mode)
            {
                case MOVEMENT_MODE.DIRECTION:
                    AddComponent(entity, new Config_Movement_Direction());
                    break;

                case MOVEMENT_MODE.NAV_AGENT:
                    AddComponent(entity, new Config_Movement_NavAgent());
                    break;
            }
        }

        private void Add_ClusterMode(ref Entity entity, CLUSTER_MODE mode)
        {
            switch (mode)
            {
                case CLUSTER_MODE.BOIDS:
                    AddComponent(entity, new Config_Cluster_BOIDS());
                    break;
            }
        }

        private void Add_FindTargetMode(ref Entity entity, FIND_TARGET_MODE mode)
        {
            switch (mode)
            {
                case FIND_TARGET_MODE.LINEAR_SEARCH:
                    AddComponent(entity, new Config_FindTarget_LinearSearch());
                    break;
            }
        }
    }

    #region [ Default ]

    public struct Config_Info
         : IComponentData
    {
        public MOVEMENT_MODE    movementMode;
        public CLUSTER_MODE     clusterMode;
        public FIND_TARGET_MODE findTargetMode;
    }

    #endregion

    #region [ Movement ]

    public enum MOVEMENT_MODE
    {
        NONE,

        DIRECTION,
        NAV_AGENT
    }

    public struct Config_Movement_Direction
        : IComponentData
    { }

    public struct Config_Movement_NavAgent
        : IComponentData
    { }

    #endregion

    #region [ Cluster ]

    public enum CLUSTER_MODE
    {
        NONE,

        BOIDS,
    }

    public struct Config_Cluster_BOIDS
        : IComponentData
    { }

    #endregion

    #region [ Find Target ]

    public enum FIND_TARGET_MODE
    {
        NONE,

        LINEAR_SEARCH
    }

    public struct Config_FindTarget_LinearSearch
        : IComponentData
    { }

    #endregion
}
