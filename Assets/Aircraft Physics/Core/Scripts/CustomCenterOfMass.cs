using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Aircraft_Physics.Core.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCenterOfMass : MonoBehaviour
    {
        private List<ColliderDensity> _colliderDensities;

        private float _totalMass;
        private Vector3 _weightedCenter;

        private Vector3 _centerOfMass;
        // Start is called before the first frame update
        private void Start()
        {
            //gets both the colliders in children and in self
            _colliderDensities = GetComponentsInChildren<ColliderDensity>().ToList();
            var collidersOnSelf = GetComponents<ColliderDensity>();
            foreach (var colliderDensity in collidersOnSelf)
            {
                _colliderDensities.Add(colliderDensity);
            }
            
            //get the total mass and 
            foreach (var colliderDensity in _colliderDensities)
            {
                _totalMass += colliderDensity.mass;
                _weightedCenter += colliderDensity.Center * colliderDensity.mass;
            }

            _centerOfMass = _weightedCenter / _totalMass;
            //GetComponent<Rigidbody>().centerOfMass = _centerOfMass;
        }
    }
}
