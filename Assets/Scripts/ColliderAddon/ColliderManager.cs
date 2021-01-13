using System;
using System.Collections.Generic;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using NUnit.Framework;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using TypeReferences;
using UnityEditor;

namespace ColliderAddon
{
    [ExecuteAlways]
    [RequireComponent(typeof(Rigidbody))]
    public class ColliderManager : MonoBehaviour
    {
        [SerializeField]
        [Inherits(typeof(Enum))]
        private TypeReference typeEnum;
        public TypeReference TypeEnum => typeEnum;
        
        [SerializeField]
        [Label("ColliderInfo Folder Path")]
        private string colliderInfoFolderPath = "Assets/Scripts/ColliderInfo";

        [SerializeField] 
        [OnValueChanged("ChangeDefaultMass")]
        [OnValueChanged("UpdateExtendedColliderDensity")]
        private float defaultMass;
        private void ChangeDefaultMass()
        {
            var massWithoutWheels = defaultMass;
            foreach (var thisCollider in colliders)
            {
                if (thisCollider.collider is WheelCollider wheelCollider)
                {
                    massWithoutWheels -= wheelCollider.mass;
                }
            }
            defaultDensity = ColliderUtil.GetColliderDensity(colliders[0].collider, massWithoutWheels);
        }

        public float DefaultMass => defaultMass;
        
        [SerializeField] 
        [OnValueChanged("ChangeDefaultDensity")]
        [OnValueChanged("UpdateExtendedColliderDensity")]
        private float defaultDensity;
        private void ChangeDefaultDensity() => defaultMass = ColliderUtil.GetRigidBodyVolume(_rb) * defaultDensity;
        public float DefaultDensity => defaultDensity;
        private void UpdateExtendedColliderDensity()
        {
            foreach (var thisCollider in colliders)
            {
                thisCollider.density = defaultDensity;
            }
        }

        [SerializeField]
        [ReorderableList] 
        private List<ExtendedCollider> colliders;
        public List<ExtendedCollider> Colliders => colliders;
    
        //dictionary to make finding colliders easy
        private Dictionary<Collider, ExtendedCollider> _extendedColliderDictionary;
        public Dictionary<Collider, ExtendedCollider> ExtendedColliderDictionary => _extendedColliderDictionary;
        
        private Vector3 _centerOfMass;
        private Rigidbody _rb;

        private void Start()
        {
            _extendedColliderDictionary = colliders.ToDictionary(thisCollider => thisCollider.collider);
            foreach (var thisCollider in colliders)
            {
                thisCollider.Start(this);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateCenterOfMass();
            }
        }
    
        //gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (colliders.Count == 0)
            {
                return;
            }
            foreach (var extendedCollider in colliders)
            {
                DrawColliderInfoGizmos(extendedCollider);
            }
            
            //COM
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.TransformPoint(_centerOfMass), 0.2f);
        }

        private void DrawColliderInfoGizmos(ExtendedCollider extendedCollider)
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(extendedCollider.Center), 0.1f);
        }

        [Button("Generate ExtendedColliders")]
        private void GenerateExtendedCollider()
        { 
            // TypeReference MyEnumType = typeEnum;
            // var temp = Enum.GetValues(MyEnumType).Cast<Enum>();
            // Debug.Log("Enum Values: " + temp.Count() + temp.FirstOrDefault());
            
            //find the colliders that need to be added
            var allColliders = GetComponentsInChildren<Collider>();
            foreach (var childCollider in allColliders)
            {
                if (_extendedColliderDictionary.ContainsKey(childCollider) || 
                    childCollider.attachedRigidbody == null)
                {
                    continue;
                }
                
                //name to use for ColliderInfo and ExtendedCollider
                var colliderType = childCollider.GetType();
                var colliderName = colliderType.Name + " #" + (colliders.Count(thisCollider => thisCollider.collider.GetType() == colliderType) + 1);

                //create new colliderInfo
                var newColliderInfo = ScriptableObjectUtil.CreateScriptableObjectInstance<ColliderInfo>
                    (colliderInfoFolderPath + "/ColliderInfo_" + colliderName + ".asset", false);

                //create new extendedCollider
                var newExtendedCollider =  new ExtendedCollider(
                    colliderName, newColliderInfo, childCollider, this);
                colliders.Add(newExtendedCollider);
                
                //update dictionary
                _extendedColliderDictionary.Add(childCollider, newExtendedCollider);
            }
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
                ScriptableObjectUtil.DeleteScriptableObjectInstance(extendedCollider.config);
            }
            colliders.RemoveAll((item) => true);
            _extendedColliderDictionary.Clear();

        UpdateCenterOfMass();
        }
        
        public void UpdateCenterOfMass()
        {
            _rb = GetComponent<Rigidbody>();
        
            //update extendedColliders
            if (!Application.isPlaying)
            {
                foreach (var thisCollider in colliders)
                {
                    thisCollider.EditorTick();
                }
            }

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
                totalMass += collider.Mass;
                weightedCenter += collider.Center * collider.Mass;
            }

            mass = totalMass;
            return weightedCenter / mass;
        }
    }

    [Serializable]
    public class ColliderInfoDictionary : SerializableDictionaryBase<Collider, ColliderInfo> { }
}