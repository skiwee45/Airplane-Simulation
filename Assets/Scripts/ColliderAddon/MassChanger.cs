using System;
using System.Linq;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using UnityEngine;

namespace ColliderAddon
{
    [RequireComponent(typeof(ColliderManager))]
    public class MassChanger : MonoBehaviour
    {
        private ColliderManager _colliderManager;

        private void Start()
        {
            _colliderManager = GetComponent<ColliderManager>();
        }
        
        public void SetMassAboveMinimum(AirplaneColliderType colliderType, float extraMass)
        {
            var collidersToChange =
                _colliderManager.Colliders.Where(thisCollider =>
                    thisCollider.config.type.ToString() == colliderType.ToString());
            foreach (var extendedCollider in collidersToChange)
            {
                SetMass(extendedCollider, extendedCollider.config.minimumMass + extraMass);
            }
        }
        
        
        public void SetMass(AirplaneColliderType colliderType, float mass)
        {
            var collidersToChange =
                _colliderManager.Colliders.Where(thisCollider =>
                    thisCollider.config.type.ToString() == colliderType.ToString());
            foreach (var extendedCollider in collidersToChange)
            {
                SetMass(extendedCollider, mass);
            }
        }

        public void SetMass(ExtendedCollider extendedCollider, float mass)
        {
            extendedCollider.Mass = mass;
        }
    }
    
    public static class MassScalarType {
        public const float Person = 75f;
        public const float Fuel = 2.4528f;
    }
}