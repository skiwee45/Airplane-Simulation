using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using UnityEngine;

namespace ColliderAddon
{
    [CreateAssetMenu(fileName = "New Collider Info", menuName = "Scriptable Object/Collider Information", order = 0)]
    public class ColliderInfo : ScriptableObject
    {
        public Collider collider;
        private Transform _transform;
        public AirplaneColliderType type;
        [Range(0, 100)] public int importance;

        //Center of Mass Related
        public float minimumMass;
        public float currentMass;
        public float maximumMass;


        [OnValueChanged("OnValidate")] 
        public Vector3 localCenter;
        public Vector3 center;
        [ReadOnly]
        public Vector3 globalCenter;
    
        private void OnValidate()
        {
            importance = Mathf.Clamp(importance, 0, 100);
            currentMass = Mathf.Clamp(currentMass, minimumMass, maximumMass);
            globalCenter = _transform.TransformPoint(localCenter);
        }
    
        public void SetDefaultValues()
        {
            _transform = collider.gameObject.transform;
            importance = Mathf.RoundToInt(ColliderUtil.GetColliderVolumePercent(collider));
        
            minimumMass = 0f;
            currentMass = ColliderUtil.GetColliderVolume(collider);
            maximumMass = currentMass * 2f;
        
            localCenter = ColliderUtil.GetColliderCenter(collider);
        }

        
    }
}