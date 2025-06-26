using UnityEngine;

using Unity.Entities;

public class HandAuthoring : MonoBehaviour
{
    [Header("\n[ HandData ]")]
    public GameObject       owner;
    public float            offsetRange;

    [Header("\n[ SearchAngleData ]")]
    public float            searchRange;
    public float            findAngle_begin;
    public float            findAngle_end;

    class Baker : Baker<HandAuthoring> 
    {
        public override void Bake(HandAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new HandData
            {
                owner           = GetEntity(authoring.owner, TransformUsageFlags.None),
                offsetRange     = authoring.offsetRange,
            });
            AddComponent(entity, new SearchAngleData
            {
                searchRange     = authoring.searchRange,
                findAngle_begin = authoring.findAngle_begin,
                findAngle_end   = authoring.findAngle_end,
            });
        }
    }

    public struct HandData
        : IComponentData
    {
        public Entity   owner;
        public float    offsetRange;
    }
}
