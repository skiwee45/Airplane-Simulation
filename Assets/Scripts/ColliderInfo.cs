using System;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collider Info", menuName = "Scriptable Object/Collider Information", order = 0)]
public class ColliderInfo : ScriptableObject
{
    public AirplaneColliderType type;
    public int importance;
    public float initialWeight;

    private void OnValidate()
    {
        importance = Mathf.Clamp(importance, 0, 10);
    }
}