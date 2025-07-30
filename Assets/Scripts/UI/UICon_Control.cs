using UnityEngine;
using UnityEngine.UI;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class UICon_Control : MonoBehaviour
{
    [Header("\n[ UI ]")]
    [SerializeField]
    private Joystick joystick;

    [Header("\n[ Camera ]")]
    [SerializeField]
    private Vector3 camOffsetPos;
    [SerializeField]
    private Vector3 camOffsetRot;

    private EntityManager   entityManager;
    private EntityQuery     followTargetQuery;
    private Entity          inputEntity;
    private bool            isJump = false;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!entityManager.Exists(inputEntity))
        {
            inputEntity = entityManager.CreateEntity(typeof(JoystickInputData));
        }

        followTargetQuery = entityManager.CreateEntityQuery
        (
            ComponentType.ReadOnly<InputData>(),
            ComponentType.ReadOnly<LocalTransform>()
        );
    }

    private void Update()
    {
        if (joystick == null)
            return;

        if (!entityManager.Exists(inputEntity) ||
            entityManager.HasComponent<LocalToWorld>(inputEntity) ||
            followTargetQuery.IsEmpty)
            return;

        // 카메라
        Entity         targetEntity = followTargetQuery.GetSingletonEntity();
        LocalTransform transform    = entityManager.GetComponentData<LocalTransform>(targetEntity);

        Vector3    targetPos = (Vector3)transform.Position + camOffsetPos;
        Quaternion targetRot = Quaternion.Euler(camOffsetRot);

        Camera.main.transform.position = Vector3.Lerp
        (
            Camera.main.transform.position,
            targetPos,
            Time.deltaTime * 5f
        );
        Camera.main.transform.rotation = Quaternion.Slerp
        (
            Camera.main.transform.rotation,
            targetRot,
            Time.deltaTime * 5f
        );
        
        // 엔티티
        float2 dir = new float2
        (
            joystick.Horizontal, 
            joystick.Vertical
        );

        entityManager.SetComponentData(inputEntity, new JoystickInputData
        {
            dir     = dir,
            jump    = isJump
        });

        if (isJump)
        { 
            isJump = false;
        }
    }

    public void OnClick_Jump()
    {
        isJump = true;
    }
}
