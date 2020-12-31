using System;
using UnityEngine;
using Object = System.Object;

namespace Aircraft_Physics.Core.Scripts.CenterOfMass
{
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
            if (OnParametersChanged != null)
            { 
                Debug.Log("Collider Density " + mass);
                OnParametersChanged();
            }
        }

        [SerializeField]
        private float minMass;
        public float GetMinMass() => minMass;
        public void SetMinMass(float value)
        {
            minMass = Mathf.Clamp(value, Mathf.NegativeInfinity, maxMass);
            SetMass(Mathf.Clamp(mass, minMass, maxMass));
        }

        [SerializeField]
        private float maxMass;
        public float GetMaxMass() => maxMass;
        public void SetMaxMass(float value)
        {
            maxMass = Mathf.Clamp(value, minMass, Mathf.Infinity);
            SetMass(Mathf.Clamp(mass, minMass, maxMass));
        }

        [field: SerializeField]
        public Vector3 LocalCenter { get; private set; } //relative to gameobject the colliderdensity is on
        public Vector3 GlobalCenter { get; set; } //world position of center
        public Vector3 RelativeCenter { get; set; } //relative to gameobject the custom center of mass is on
        
        [SerializeField] private Collider assignedCollider = null;
        
        //event
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnParametersChanged;
        public void SetupEvent(CustomCenterOfMass ccom)
        {
            ccom.OnUpdateCenterOfMass += Initiate;
        }

        private void OnValidate()
        {
            Initiate();
            if (OnParametersChanged != null)
            {
                OnParametersChanged();
            }
        }

        private void Initiate()
        {
            mass = Mathf.Clamp(mass, minMass, maxMass);
            GlobalCenter = LocalToGlobalPosition(transform, LocalCenter);
        }

        private void Awake()
        {
            //get the center from the collider if collider is attached
            // if (!assignedCollider.IsNull())
            // {
            //     LocalCenter = GetCenterFromCollider(assignedCollider);
            // }
            Initiate();
        }
        
        //gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GlobalCenter, 0.1f);
        }

        public Vector3 LocalToGlobalPosition(Transform relativeTransform, Vector3 localPosition)
        {
            return relativeTransform.TransformPoint(localPosition);
        }
        
        public Vector3 GlobalToLocalPosition(Transform relativeTransform, Vector3 globalPosition)
        {
            return relativeTransform.InverseTransformPoint(globalPosition);
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