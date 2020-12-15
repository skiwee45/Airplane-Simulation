using UnityEngine;

namespace Aircraft_Physics.Core.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCenterOfMass : MonoBehaviour
    {
        private ColliderDensity[] _colliderDensities;

        private float _mass;
        private Vector3 _center;

        private Vector3 _centerOfMass;
        // Start is called before the first frame update
        private void Start()
        {
            _colliderDensities = GetComponentsInChildren<ColliderDensity>();
            foreach (var colliderDensity in _colliderDensities)
            {
                _mass += colliderDensity.mass;
                _center += colliderDensity.Center;
            }

            _centerOfMass = _center / _mass;
            GetComponent<Rigidbody>().centerOfMass = _centerOfMass;
        }
    }
}
