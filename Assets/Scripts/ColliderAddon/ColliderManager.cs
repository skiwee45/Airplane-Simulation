using System;
using System.Collections.Generic;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using NUnit.Framework;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace ColliderAddon
{
    [RequireComponent(typeof(Rigidbody))]
    public class ColliderManager : MonoBehaviour
    {
        [SerializeField]
        [Label("ColliderInfo Folder Path")]
        private string colliderInfoFolderPath = "Assets/Scripts/ColliderInfo";
        
        [SerializeField]
        [ReorderableList] 
        private List<ColliderInfo> colliders;
        public List<ColliderInfo> Colliders => colliders;
    
        //dictionary to make finding colliders easy
        private Dictionary<Collider, ColliderInfo> _colliderInfoDictionary;
    
        private Vector3 _centerOfMass;
        [SerializeField]
        [Required()]
        private Rigidbody rb;

        private void Start()
        {
            
        }

        private void OnValidate()
        {
            //generate dictionary
            _colliderInfoDictionary = colliders.ToDictionary(info => info.collider);
            //update COM
            UpdateCenterOfMass();
        }
    
        //gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var colliderInfo in colliders)
            {
                DrawColliderInfoGizmos(colliderInfo);
            }
        }

        private void DrawColliderInfoGizmos(ColliderInfo colliderInfo)
        {
            Gizmos.DrawWireSphere(colliderInfo.globalCenter, 0.1f);
        }

        [Button("Generate ColliderInfo")]
        private void GenerateColliderInfo()
        {
            //find the colliders that need to be added
            var allColliders = GetComponentsInChildren<Collider>();
            foreach (var collider in allColliders)
            {
                if (!_colliderInfoDictionary.ContainsKey(collider)) //does not have colliderInfo yet
                {
                    //create new colliderinfo and add to list
                    var obj = ScriptableObjectUtil.CreateScriptableObjectInstance<ColliderInfo>
                        (colliderInfoFolderPath + "/New Collider Info.asset");
                    colliders.Add(obj);
                    
                    //put collider in
                    obj.collider = collider;
                }
            }
            //since they all have colliders, all other information can be generated
            DefaultColliderInfo();
        }
    
        private void ColliderCenterCalculation()
        {
            foreach (var colliderInfo in colliders)
            {
                colliderInfo.center = transform.InverseTransformPoint(colliderInfo.globalCenter);
            }
        }

        [Button("Reset ColliderInfo to Default")]
        private void DefaultColliderInfo()
        {
            foreach (var colliderInfo in colliders)
            {
                colliderInfo.SetDefaultValues();
            }
            ColliderCenterCalculation();
        }
    
        private void UpdateCenterOfMass()
        {
            rb = GetComponent<Rigidbody>();
            //make sure all colliders have the right centers
            ColliderCenterCalculation();
        
            //calculations
            _centerOfMass = CalculateCenterOfMass(colliders, out var totalMass);
            Debug.Log("UpdateCOM " + totalMass);

            //set COM and Mass
            rb.mass = totalMass;
            rb.centerOfMass = _centerOfMass;
        }

        public static Vector3 CalculateCenterOfMass(List<ColliderInfo> colliders, out float mass)
        {
            var totalMass = 0f;
            var weightedCenter = new Vector3(0, 0, 0);
            //get the total mass and weighted center
            foreach (var collider in colliders)
            {
                totalMass += collider.currentMass;
                weightedCenter += collider.center * collider.currentMass;
            }

            mass = totalMass;
            return weightedCenter / mass;
        }
    }

    [Serializable]
    public class ColliderInfoDictionary : SerializableDictionaryBase<Collider, ColliderInfo> { }
}