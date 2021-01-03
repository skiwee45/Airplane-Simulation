using System;
using NaughtyAttributes;
using UnityEngine;

namespace ColliderAddon
{
    [System.Serializable]
    public class ExtendedCollider
    {
        //fields
        public String name;
        public float currentMass;
        public Collider collider;
        public ColliderInfo config;
        
        private Transform _transform;
        private ColliderManager _parent;
        
        //event
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnExtendedColliderChanged;
        public ExtendedCollider(ColliderInfo config, Collider collider, ColliderManager parent, bool defaultValues)
        {
            this.config = config;
            this.collider = collider;
            _parent = parent;
            _transform = collider.transform;
            this.config.OnColliderInfoChanged += OnUpdateFields;
            
            //default it if wanted
            if (defaultValues)
            {
                SetDefaultValues();
            }
        }
        
        public void OnCreate()
        {
            OnExtendedColliderChanged += _parent.UpdateCenterOfMass;
            config.OnColliderInfoChanged += OnUpdateFields;
        }

        public void OnDelete()
        {
            OnExtendedColliderChanged -= _parent.UpdateCenterOfMass;
            config.OnColliderInfoChanged -= OnUpdateFields;
        }
        
        private void OnUpdateFields()
        {
            Debug.Log("Fields Updated");
            if (OnExtendedColliderChanged != null)
            {
                OnExtendedColliderChanged();
            }
            _transform = collider.transform;
            config.importance = Mathf.Clamp(config.importance, 0, 100);
            currentMass = Mathf.Clamp(currentMass, config.minimumMass, config.maximumMass);
            config.globalCenter = _transform.TransformPoint(config.localCenter);
            config.center = _parent.transform.InverseTransformPoint(config.globalCenter);
        }
    
        public void SetDefaultValues()
        {
            name = "ExtendedCollider #" + collider.GetInstanceID();
            config.importance = Mathf.RoundToInt(ColliderUtil.GetColliderVolumePercent(collider));
        
            config.minimumMass = 0f;
            currentMass = ColliderUtil.GetColliderVolume(collider);
            config.maximumMass = currentMass * 2f;
        
            config.localCenter = ColliderUtil.GetColliderCenter(collider);
        }
    }
}