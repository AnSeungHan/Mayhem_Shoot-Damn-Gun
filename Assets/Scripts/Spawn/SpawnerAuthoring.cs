using UnityEngine;

using Unity.Entities;

public class SpawnerAuthoring : MonoBehaviour
{
    [Header("\n[ Prefabs ]")]
    public GameObject   createPrefab;

    [Header("\n[ Setting ]")]
    public int          numCreate;
    public Vector2      bounds;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);

            AddComponent(entity, new SpawnerData 
            {
                createPrefab    = GetEntity(authoring.createPrefab, TransformUsageFlags.Dynamic),
                numCreate       = authoring.numCreate,
                bounds          = authoring.bounds,
            });
        }
    }
}
