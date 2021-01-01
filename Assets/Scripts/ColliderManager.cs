using System.Collections.Generic;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    [SerializeField]
    private Dictionary<AirplaneColliderType, Collider> _colliders;
}