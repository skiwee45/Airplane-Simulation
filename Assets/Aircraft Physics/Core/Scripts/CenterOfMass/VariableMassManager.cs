using System.Collections.Generic;
using System.Linq;
using Pixelplacement;
using UnityEngine;

namespace Aircraft_Physics.Core.Scripts.CenterOfMass
{
    [RequireComponent(typeof(CustomCenterOfMass))]
    public class VariableMassManager : Singleton<VariableMassManager>
    {
        private CustomCenterOfMass _customCenterOfMass;
        private Dictionary<ColliderDensityType, ColliderDensity> _colliderDensities;

        private void Start()
        {
            _customCenterOfMass = GetComponent<CustomCenterOfMass>();
            _colliderDensities = _customCenterOfMass.colliderDensities.ToDictionary(colliderDensity => colliderDensity.colliderType);
        }

        public void WingMass(float fuel)
        {
            SetMass(ColliderDensityType.Wings, fuel * MassScalarType.Fuel);
        }

        public void PeopleMass(int people)
        {
            SetMass(ColliderDensityType.FuselageFront, people * MassScalarType.Person);
        }
        
        private void SetMass(ColliderDensityType colliderType, float extraMass)
        {
            var colliderDensity = _colliderDensities[colliderType];
            if (!colliderDensity) return;
            Debug.Log("SetMass " + extraMass + "\tColliderType: " + colliderType);
            colliderDensity.SetMass(extraMass + colliderDensity.GetMinMass());
        }
    }
    
    public static class MassScalarType {
        public const float Person = 75f;
        public const float Fuel = 2.4528f;
    }
}