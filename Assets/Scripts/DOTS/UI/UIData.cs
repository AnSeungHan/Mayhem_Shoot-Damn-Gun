using UnityEngine;

using Unity.Entities;

public struct ButtonClickState 
    : IComponentData
{
    public bool WasClicked;
}
