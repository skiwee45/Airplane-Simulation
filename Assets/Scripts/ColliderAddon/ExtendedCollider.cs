using System;
using NaughtyAttributes;
using UnityEngine;

namespace ColliderAddon
{
    [System.Serializable]
    public class ExtendedCollider
    {
        //fields
        public string name;
        public float currentMass;
        public Collider collider;
        public ColliderInfo config;
        
        private Transform _transform;
        private Transform _parentTransform;
        private ColliderManager _parent;
        private float _density;
        
        //event
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnExtendedColliderChanged;
        public ExtendedCollider(string name, ColliderInfo config, Collider collider, ColliderManager parent, float density, bool defaultValues)
        {
            this.name = name;
            this.config = config;
            this.collider = collider;
            _transform = collider.transform;
            _parent = parent;
            _parentTransform = parent.transform;
            _density = density;
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
            OnExtendedColliderChanged?.Invoke();
            _transform = collider.transform;
            config.importance = Mathf.Clamp(config.importance, 0, 100);
            currentMass = Mathf.Clamp(currentMass, config.minimumMass, config.maximumMass);
            config.globalCenter = _transform.TransformPoint(config.localCenter);
            config.center = _parentTransform.InverseTransformPoint(config.globalCenter);
        }
    
        public void SetDefaultValues()
        {
            config.importance = Mathf.RoundToInt(ColliderUtil.GetColliderVolumePercent(collider));
        
            config.minimumMass = 0f;
            currentMass = ColliderUtil.GetColliderVolume(collider) * _density;
            config.maximumMass = currentMass * 2f;
        
            config.localCenter = ColliderUtil.GetColliderCenter(collider);
        }
    }
}