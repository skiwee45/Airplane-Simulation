using System;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using NaughtyAttributes;
using UnityEngine;

namespace ColliderAddon
{
    [ExecuteAlways]
    [CreateAssetMenu(fileName = "New Collider Info", menuName = "Scriptable Object/Collider Information", order = 0)]
    public class ColliderInfo : ScriptableObject
    {
        public AirplaneColliderType type;
        [Range(0, 100)] 
        public int startingImportance;

        //Center of Mass Related
        [BoxGroup("Center of Mass")]
        public float minimumMass;
        [BoxGroup("Center of Mass")]
        [OnValueChanged("ClampMass")]
        public float startingMass;
        private void ClampMass() => startingMass = Mathf.Clamp(startingMass, minimumMass, maximumMass);
        [BoxGroup("Center of Mass")]
        public float maximumMass;
        
        [BoxGroup("Center of Mass")]
        public Vector3 localCenter;
        [BoxGroup("Center of Mass")] [ReadOnly]
        public Vector3 globalCenter;
    }
}