using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Aircraft_Physics.Core.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCenterOfMass : MonoBehaviour
    {
        public List<ColliderDensity> colliderDensities;
        private Vector3 _centerOfMass;
        private Rigidbody _rb;
        
        // Start is called before the first frame update
        private void Awake()
        {
            //gets both the colliders in children and in self
            colliderDensities = GetComponentsInChildren<ColliderDensity>().ToList();
            var collidersOnSelf = GetComponents<ColliderDensity>();
            foreach (var colliderDensity in collidersOnSelf)
            {
                colliderDensities.Add(colliderDensity);
            }
            
            //add update event
            foreach (var colliderDensity in colliderDensities)
            {
                colliderDensity.MassChanged += UpdateCenterOfMass;
            }
            
            //make sure centers are relative to the gameobject, not the collider gameobject children
            AdjustColliderCenters(transform, ref colliderDensities);
            
            _rb = GetComponent<Rigidbody>();

            UpdateCenterOfMass();
        }

        private void UpdateCenterOfMass()
        {
            //calculations
            _centerOfMass = CalculateCenterOfMass(colliderDensities, out var totalMass);
            Debug.Log("UpdateCOM " + totalMass);

            //set COM and Mass
            //_rb.mass = totalMass;
            //_rb.centerOfMass = _centerOfMass;
        }

        public static Vector3 CalculateCenterOfMass(List<ColliderDensity> colliderDensities, out float mass)
        {
            var totalMass = 0f;
            var weightedCenter = new Vector3(0, 0, 0);
            //get the total mass and weighted center
            foreach (var colliderDensity in colliderDensities)
            {
                totalMass += colliderDensity.GetMass();
                weightedCenter += colliderDensity.RelativeCenter * colliderDensity.GetMass();
            }

            mass = totalMass;
            return weightedCenter / mass;
        }

        public static void AdjustColliderCenters(Transform origin, ref List<ColliderDensity> colliderDensities)
        {
            foreach (var colliderDensity in colliderDensities)
            {
                var globalLocation = colliderDensity.transform.TransformPoint(colliderDensity.Center);
                var relativeLocation = origin.InverseTransformPoint(globalLocation);
                colliderDensity.RelativeCenter = relativeLocation;
            }
        }
    }
}
