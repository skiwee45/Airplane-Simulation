using System;
using System.Collections.Generic;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    [SerializeField]
    private ColliderInfoDictionary colliders;

    public ColliderInfoDictionary Colliders => colliders;

    [Button]
    private void GenerateColliderInfo()
    {
        
    }
}

[Serializable]
public class ColliderInfoDictionary : SerializableDictionaryBase<Collider, ColliderInfo> { }