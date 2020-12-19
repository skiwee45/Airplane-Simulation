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

        public Vector3 Center { get; private set; }
        public Vector3 RelativeCenter { get; set; }

        [SerializeField] private Collider assignedCollider = null;
        
        //event
        public UnityEvent massChanged;

        public delegate void ChangeMass();
        public event ChangeMass MassChanged;

        private void Start()
        {
            massChanged = new UnityEvent();
            
            if (assignedCollider.IsNull())
            {
                assignedCollider = GetComponent<Collider>();
            }

            switch (assignedCollider)
            {
                case BoxCollider boxCollider:
                    Center = boxCollider.center;
                    break;
                case CapsuleCollider capsuleCollider:
                    Center = capsuleCollider.center;
                    break;
                case SphereCollider sphereCollider:
                    Center = sphereCollider.center;
                    break;
                case WheelCollider wheelCollider:
                    Center = wheelCollider.center;
                    break;
                default:
                    Center = assignedCollider.bounds.center;
                    break;
            }
        }
    }
}