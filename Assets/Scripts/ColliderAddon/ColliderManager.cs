using System;
using System.Collections.Generic;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using NUnit.Framework;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using TypeReferences;

namespace ColliderAddon
{
    [RequireComponent(typeof(Rigidbody))]
    public class ColliderManager : MonoBehaviour
    {
        [SerializeField]
        [Inherits(typeof(Enum))]
        private TypeReference typeEnum;
        
        [SerializeField]
        [Label("ColliderInfo Folder Path")]
        private string colliderInfoFolderPath = "Assets/Scripts/ColliderInfo";

        [SerializeField] 
        private float defaultDensity = 50f;
        
        [SerializeField]
        [ReorderableList] 
        private List<ExtendedCollider> colliders;
        public List<ExtendedCollider> Colliders => colliders;
    
        //dictionary to make finding colliders easy
        [SerializeField]
        private Dictionary<Collider, ExtendedCollider> _extendedColliderDictionary = new Dictionary<Collider, ExtendedCollider>(0);
        public Dictionary<Collider, ExtendedCollider> ExtendedColliderDictionary => _extendedColliderDictionary;
        
        private Vector3 _centerOfMass;
        private Rigidbody _rb;

        private void OnValidate()
        {
            Debug.Log("Manager Updated");
            //update COM
            UpdateCenterOfMass();
            //generate dictionary
            _extendedColliderDictionary = colliders.ToDictionary(extendedCollider => extendedCollider.collider);
        }
    
        //gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (colliders.Count > 0)
            {
                foreach (var extendedCollider in colliders)
                {
                    DrawColliderInfoGizmos(extendedCollider);
                }
            }
        }

        private void DrawColliderInfoGizmos(ExtendedCollider extendedCollider)
        {
            Gizmos.DrawWireSphere(extendedCollider.config.globalCenter, 0.1f);
        }

        [Button("Generate ExtendedColliders")]
        private void GenerateExtendedCollider()
        { 
            //find the colliders that need to be added
            var allColliders = GetComponentsInChildren<Collider>();
            foreach (var childCollider in allColliders)
            {
                // Debug.Log("Collider ID: " + colliderID);
                // var temp = childCollider.attachedRigidbody?.mass;
                // Debug.Log("Nullable Type Test" + temp);
                // Debug.Log("dict: " + _extendedColliderDictionary);
                if (_extendedColliderDictionary.ContainsKey(childCollider) || 
                    childCollider.attachedRigidbody == null)
                {
                    continue;
                }
                
                //name to use for ColliderInfo and ExtendedCollider
                var colliderName = childCollider.GetType().Name + " #" + (colliders.Count + 1);

                //create new colliderInfo
                var newColliderInfo = ScriptableObjectUtil.CreateScriptableObjectInstance<ColliderInfo>
                    (colliderInfoFolderPath + "/ColliderInfo_" + colliderName + ".asset", false);

                //create new extendedCollider
                var newExtendedCollider =  new ExtendedCollider(
                    colliderName, newColliderInfo, childCollider, this, defaultDensity, true);
                colliders.Add(newExtendedCollider);
                newExtendedCollider.OnCreate();
                
                //update dictionary
                _extendedColliderDictionary.Add(childCollider, newExtendedCollider);
            }
            //Debug.Log("Colliders List" + colliders);
            UpdateCenterOfMass();
        }
    
        
        [Button("Reset ColliderInfo to Default Values")]
        private void DefaultColliderInfo()
        {
            foreach (var extendedCollider in colliders)
            {
                extendedCollider.SetDefaultValues();
            }
            UpdateCenterOfMass();
        }

        [Button("Delete ExtendedColliders")]
        private void DeleteExtendedColliders()
        {
            foreach (var extendedCollider in colliders)
            {
                extendedCollider.OnDelete();
                ScriptableObjectUtil.DeleteScriptableObjectInstance(extendedCollider.config);
            }
            colliders.RemoveAll((ExtendedCollider item) => true);
            _extendedColliderDictionary.Clear();

        UpdateCenterOfMass();
        }
    
        public void UpdateCenterOfMass()
        {
            _rb = GetComponent<Rigidbody>();
        
            //calculations
            _centerOfMass = CalculateCenterOfMass(colliders, out var totalMass);
            Debug.Log("UpdateCOM " + totalMass);

            //set COM and Mass
            _rb.mass = totalMass;
            _rb.centerOfMass = _centerOfMass;
        }

        public static Vector3 CalculateCenterOfMass(List<ExtendedCollider> colliders, out float mass)
        {
            var totalMass = 0f;
            var weightedCenter = new Vector3(0, 0, 0);
            //get the total mass and weighted center
            foreach (var collider in colliders)
            {
                totalMass += collider.currentMass;
                weightedCenter += collider.config.center * collider.currentMass;
            }

            mass = totalMass;
            return weightedCenter / mass;
        }
    }

    [Serializable]
    public class ColliderInfoDictionary : SerializableDictionaryBase<Collider, ColliderInfo> { }
}