using UnityEngine;

using Unity.Entities;

public class UICon_Create : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity singletonEntity;

    void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void OnClick_Create()
    {
        Entity singletonEntity = entityManager.CreateEntity(typeof(ButtonClickState));
        entityManager.SetComponentData(singletonEntity, new ButtonClickState { WasClicked = false });
    }
}


