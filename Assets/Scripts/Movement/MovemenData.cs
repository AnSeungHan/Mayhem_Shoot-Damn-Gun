using Unity.Entities;
using Unity.Mathematics;

public struct MovementData
    : IComponentData
{
    public float        moveSpeed;
    public float        curSpeed;
    public float        acceleration;
    public float        angularSpeed;

    public float        stopDistance;
}


#region [ Direction ]

public struct MovementDirectionData
    : IComponentData
{ }

#endregion

#region [ Nav Agent ]

public struct NavAgentData : IComponentData
{
    public bool         isReachedDestination;
    public bool         isMove;
    public bool         isHasPath;
    public bool         isPathCalculated;
    public bool         isNeedsNewPath;
    public bool         isValid;

    public BlobAssetReference<NavMeshPathBlob> PathBlob;
    public int          currentWaypoint;
    public int          currentCornerIndex;
    public float3       prePosition;
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