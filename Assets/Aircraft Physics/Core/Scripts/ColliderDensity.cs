using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Aircraft_Physics.Core.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class ColliderDensity : MonoBehaviour
    {
        public ColliderDensityType colliderType;
        
        //mass stuff
        [SerializeField]
        private float mass;
        public float GetMass() => mass;

        public void SetMass(float value)
        {
            mass = Mathf.Clamp(value, minMass, maxMass);
            if (MassChanged != null)
            { 
                Debug.Log("Collider Density " + mass);
                MassChanged();
            }
        }

        [SerializeField]
        private float minMass;
        public float GetMinMass() => minMass;
        public void SetMinMass(float value)
        {
            minMass = Mathf.Clamp(value, Mathf.NegativeInfinity, maxMass);
            mass = Mathf.Clamp(mass, minMass, maxMass);
        }

        [SerializeField]
        private float maxMass;
        public float GetMaxMass() => maxMass;
        public void SetMaxMass(float value)
        {
            maxMass = Mathf.Clamp(value, minMass, Mathf.Infinity);
            mass = Mathf.Clamp(mass, minMass, maxMass);
        }

        [field: SerializeField]
        public Vector3 LocalCenter { get; private set; }
        public Vector3 Center { get; set; }

        [SerializeField] private Collider assignedCollider = null;
        
        //event
        public delegate void ChangeMass();
        public event ChangeMass MassChanged;

        private void Start()
        {
            if (!assignedCollider.IsNull())
            {
                LocalCenter = GetCenterFromCollider(assignedCollider);
            }
        }

        private Vector3 GetCenterFromCollider(Collider collider)
        {
            Vector3 center;
            switch (collider)
            {
                case BoxCollider boxCollider:
                    center = boxCollider.center;
                    break;
                case CapsuleCollider capsuleCollider:
                    center = capsuleCollider.center;
                    break;
                case SphereCollider sphereCollider:
                    center = sphereCollider.center;
                    break;
                case WheelCollider wheelCollider:
                    center = wheelCollider.center;
                    break;
                default:
                    center = collider.bounds.center;
                    break;
            }

            return center;
        }
    }
}