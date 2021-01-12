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
        [BoxGroup("Runtime")]
        [ReadOnly]
        private float currentMass;
        [SerializeField]
        [BoxGroup("Runtime")]
        [ReadOnly]
        private float currentImportance;
        [SerializeField]
        [BoxGroup("Runtime")]
        [ReadOnly]
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
        
        private Vector3 _transform;
        private ColliderManager _parent;
        public float density;
        
        public ExtendedCollider(string name, ColliderInfo config, Collider collider, ColliderManager parent)
        {
            this.name = name;
            this.config = config;
            this.collider = collider;
            _transform = collider.transform.position;
            _parent = parent;
            density = parent.DefaultDensity;
            SetDefaultValues();
        }
        
        public void SetDefaultValues()
        {
            config.type = AirplaneColliderType.Wings;
            config.startingImportance = Mathf.RoundToInt(ColliderUtil.GetColliderVolumePercent(collider) * 100f);
            Importance = config.startingImportance;

            config.minimumMass = 0f;
            config.startingMass = ColliderUtil.GetColliderMass(collider, density);
            config.maximumMass = config.startingMass * 2f;
            Mass = config.startingMass;
        
            config.localCenter = ColliderUtil.GetColliderCenter(collider);
            CalculateCenter();
        }

        public void Start(ColliderManager parent)
        {
            _parent = parent;
        }

        public void EditorTick()
        {
            //update runtime variables
            Importance = config.startingImportance;
            Mass = config.startingMass;
            CalculateCenter();
        }
        
        private void CalculateCenter()
        {
            config.globalCenter = _transform + config.localCenter;
            Center = _parent.transform.InverseTransformPoint(config.globalCenter);
        }
    }
}