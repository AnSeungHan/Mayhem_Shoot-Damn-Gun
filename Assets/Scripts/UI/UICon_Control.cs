using UnityEngine;
using UnityEngine.UI;

using Unity.Entities;
using Unity.Mathematics;

public class UICon_Control : MonoBehaviour
{
    [Header("\n[ UI ]")]
    [SerializeField]
    private Joystick joystick;

    private EntityManager entityManager;
    private Entity inputEntity;
    private bool isJump = false;


    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (!entityManager.Exists(inputEntity))
        {
            inputEntity = entityManager.CreateEntity(typeof(JoystickInputData));
        }
    }

    private void FixedUpdate()
    {
        if (joystick == null)
            return;

        float2 dir = new float2(joystick.Horizontal, joystick.Vertical);

        if (entityManager.Exists(inputEntity))
        {
            entityManager.SetComponentData(inputEntity, new JoystickInputData 
            { 
                dir     = dir,
                jump    = isJump
            });

            if (isJump)
                isJump = false;
        }
    }

    public void OnClick_Jump()
    {
        isJump = true;
    }
}
