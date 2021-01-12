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
        public int importance;

        //Center of Mass Related
        [BoxGroup("Center of Mass")]
        public float minimumMass;
        [BoxGroup("Center of Mass")]
        public float maximumMass;
        
        [BoxGroup("Center of Mass")]
        public Vector3 localCenter;
        [BoxGroup("Center of Mass")]
        [ReadOnly]
        public Vector3 center;

        [BoxGroup("Center of Mass")] [ReadOnly]
        public Vector3 globalCenter;

        //event
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnColliderInfoChanged;

        private void OnValidate()
        {
            Debug.Log("ColliderInfo Updated");
            OnColliderInfoChanged?.Invoke();
        }

        public void OnSetup(ExtendedCollider parent)
        {
            if (OnColliderInfoChanged == null)
            {
                OnColliderInfoChanged += parent.OnUpdateFields;
                Debug.Log("ColliderInfo => ExtendedCollider Event Connected");
            }
        }
    }
}