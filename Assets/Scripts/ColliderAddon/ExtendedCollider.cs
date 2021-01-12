using System;
using System.Collections.Generic;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

namespace ColliderAddon
{
    [System.Serializable]
    public class ExtendedCollider
    {
        //fields
        public string name;
        public float currentMass;

        public float CurrentMass
        {
            get => currentMass;
            set
            {
                currentMass = value;
                OnUpdateFields();
            } 
        }

        public Collider collider;
        public ColliderInfo config;
        
        private Transform _transform;
        private Transform _parentTransform;
        private ColliderManager _parent;
        private float _density;
        
        //event
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnExtendedColliderChanged;
        public ExtendedCollider(string name, ColliderInfo config, Collider collider, ColliderManager parent)
        {
            this.name = name;
            this.config = config;
            this.collider = collider;
            config.OnSetup(this);
            _transform = collider.transform;
            _parent = parent;
            _parentTransform = parent.transform;
            _density = parent.DefaultDensity;
            SetDefaultValues();
        }

        public void OnUpdateFields()
        {
            Debug.Log("Fields Updated");
            _transform = collider.transform;
            config.importance = Mathf.Clamp(config.importance, 0, 100);
            currentMass = Mathf.Clamp(currentMass, config.minimumMass, config.maximumMass);
            config.globalCenter = _transform.TransformPoint(config.localCenter);
            config.center = _parentTransform.InverseTransformPoint(config.globalCenter);
            _parent.UpdateCenterOfMass();
        }
    
        public void SetDefaultValues()
        {
            config.type = AirplaneColliderType.Wings;
            config.importance = Mathf.RoundToInt(ColliderUtil.GetColliderVolumePercent(collider) * 100f);
        
            config.minimumMass = 0f;
            currentMass = ColliderUtil.GetColliderVolume(collider) * _density;
            config.maximumMass = currentMass * 2f;
        
            config.localCenter = ColliderUtil.GetColliderCenter(collider);
        }
    }
}