using System;
using System.Collections.Generic;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

namespace ColliderAddon
{
    [Serializable]
    public class ExtendedCollider
    {
        //fields
        public string name;
        [SerializeField]
        [ReadOnly]
        [BoxGroup("Runtime Info")]
        private float currentMass;
        [SerializeField]
        [ReadOnly]
        [BoxGroup("Runtime Info")]
        private float currentImportance;
        [SerializeField]
        [ReadOnly]
        [BoxGroup("Runtime Info")]
        private Vector3 currentCenter;
        public float Mass
        {
            get => currentMass;
            set => currentMass = Mathf.Clamp(value, config.minimumMass, config.maximumMass);
        }
        public float Importance
        {
            get => currentImportance;
            set => currentImportance = Mathf.Clamp(value, 0, 100);
        }
        public Vector3 Center
        {
            get => currentCenter;
            set => currentCenter = value;
        }

        public Collider collider;
        public ColliderInfo config;
        
        private Transform _transform;
        private ColliderManager _parent;
        public float density;
        
        public ExtendedCollider(string name, ColliderInfo config, Collider collider, ColliderManager parent)
        {
            this.name = name;
            this.config = config;
            this.collider = collider;
            _transform = collider.transform;
            _parent = parent;
            density = parent.DefaultDensity;
            config.type = AirplaneColliderType.Wings;
            SetDefaultValues();
        }
        
        public void SetDefaultValues()
        {
            config.startingMass = ColliderUtil.GetColliderMass(collider, density);
            config.minimumMass = Mathf.Clamp(config.minimumMass, Mathf.NegativeInfinity, config.startingMass);
            config.maximumMass = Mathf.Clamp(config.maximumMass, config.startingMass, Mathf.Infinity);
            
            config.startingImportance = Mathf.RoundToInt(ColliderUtil.GetColliderMassPercent(config.startingMass, collider.attachedRigidbody) * 100f);
        
            config.localCenter = ColliderUtil.GetColliderCenter(collider);
            
            EditorTick();
        }

        public void Start(ColliderManager parent)
        {
            _parent = parent;
            _transform = collider.transform;
        }

        public void EditorTick()
        {
            //clamp editor variables
            config.startingMass = Mathf.Clamp(config.startingMass, config.minimumMass, config.maximumMass);
            
            //update runtime variables
            Importance = config.startingImportance;
            Mass = config.startingMass;
            CalculateCenter();
        }
        
        private void CalculateCenter()
        {
            config.globalCenter = _transform.TransformPoint(config.localCenter);
            Center = _parent.transform.InverseTransformPoint(config.globalCenter);
        }
    }
}