using System.Collections.Generic;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private Dictionary<Collider, ColliderInfo> _colliders;

    public Dictionary<Collider, ColliderInfo> Colliders => _colliders;
}