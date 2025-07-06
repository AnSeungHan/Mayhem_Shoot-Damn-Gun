using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct MovementData
    : IComponentData
{
    public float        moveSpeed;
    public float        acceleration;
    public float        angularSpeed;
    public float        stopDistance;

    public float        curSpeed;
    public bool         hasNewPosition;
    public float3       moveNextPosition;
}

#region [ Direction ]

public struct MovementDirectionData
    : IComponentData
{ }

#endregion

#region [ Owner ]

public struct MovementOwnerRotationData
    : IComponentData
{
    public Entity           owner;
    public float            offset;
}

#endregion

#region [ Nav Agent ]

public struct NavAgentData : IComponentData
{
    public bool         isReachedDestination;
    public bool         isMove;
    public bool         hasPath;
    public bool         isNeedsNewPath;

    public BlobAssetReference<NavMeshPathBlob> pathBlob;
    public int          currentWaypoint;
    public int          areaMask;
    public int          currentCornerIndex;
    public float3       preTargetPosition;
}

public struct WaypointBuffer : IBufferElementData
{
    public float3 wayPoint;
}

public struct NavMeshPathBlob
{
    public BlobArray<float3> Corners;
}


#endregion

#region [ Input ]

public struct InputData
    : IComponentData
{

}

public struct JoystickInputData
    : IComponentData
{
    public float2   dir;
    public bool     jump;
}

public struct JumpInputData
    : IComponentData
{
    public float2 dir;
}


#endregion