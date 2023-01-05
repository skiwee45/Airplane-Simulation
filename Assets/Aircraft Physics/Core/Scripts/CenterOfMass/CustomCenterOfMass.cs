using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aircraft_Physics.Core.Scripts.CenterOfMass
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCenterOfMass : MonoBehaviour
    {
        public List<ColliderDensity> colliderDensities;
        private Vector3 _centerOfMass;
        private Rigidbody _rb;

        //events
        public delegate void OnFieldsChanged();
        public event OnFieldsChanged OnUpdateCenterOfMass;

        private void Awake()
        {
            //gets colliderDensities in children and in self
            colliderDensities = GetComponentsInChildren<ColliderDensity>().ToList();
            var collidersOnSelf = GetComponents<ColliderDensity>();
            foreach (var colliderDensity in collidersOnSelf)
            {
                colliderDensities.Add(colliderDensity);
            }

            //add events
            foreach (var colliderDensity in colliderDensities)
            {
                colliderDensity.OnParametersChanged += UpdateCenterOfMass;
                colliderDensity.SetupEvent(this);
            }

            _rb = GetComponent<Rigidbody>();

            UpdateCenterOfMass();
        }

        private void UpdateCenterOfMass()
        {
            if (OnUpdateCenterOfMass != null)
            {
                OnUpdateCenterOfMass();
            }
            //make sure centers are relative to the gameobject, not the collider gameobject children
            GlobalToLocalColliderCenters(transform, ref colliderDensities);

            //calculations
            _centerOfMass = CalculateCenterOfMass(colliderDensities, out var totalMass);
            Debug.Log("UpdateCOM " + totalMass);

            //set COM and Mass
            _rb.mass = totalMass;
            _rb.centerOfMass = _centerOfMass;
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

        public static void GlobalToLocalColliderCenters(Transform origin, ref List<ColliderDensity> colliderDensities)
        {
            foreach (var colliderDensity in colliderDensities)
            {
                var relativeLocation = colliderDensity.GlobalToLocalPosition(origin, colliderDensity.GlobalCenter);
                colliderDensity.RelativeCenter = relativeLocation;
            }
        }
    }
}