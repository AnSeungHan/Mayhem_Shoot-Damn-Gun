using UnityEngine;

using Unity.Entities;

using static ConfigAuthoring;

public class PlayerAuthoring : MonoBehaviour
{
    [Header("\n[ Info ]")]
    public float            moveSpeed    = 1f;
    public float            acceleration = 1f;
    public float            angularSpeed = 1f;
    public float            stopDistance = 1f;

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitData
            {
                campType        = CAMP_TYPE.ALLIANCE
            });
            AddComponent(entity, new MovementData
            {
                moveSpeed       = authoring.moveSpeed,
                acceleration    = authoring.acceleration,
                angularSpeed    = authoring.angularSpeed,
                stopDistance    = authoring.stopDistance,
            });
            AddComponent(entity, new AIData()); 
            AddComponent(entity, new TargetData());
            AddComponent(entity, new InputData());
            AddComponent(entity, new SlidWallData());
        }
    }
}

