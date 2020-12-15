using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aircraft_Physics.Core.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class ColliderDensity : MonoBehaviour
    {
        public float mass;
        public Vector3 Center { get; private set; }
        [SerializeField] private Collider collider = null;

        private void Start()
        {
            collider = collider is null ? GetComponent<Collider>() : collider;
            if (collider is BoxCollider)
            {
                Center = ((BoxCollider) collider).center;
            }
            else if (collider is CapsuleCollider)
            {
                Center = ((CapsuleCollider) collider).center;
            }
            else if (collider is SphereCollider)
            {
                Center = ((SphereCollider) collider).center;
            }
            else
            {
                Center = collider.bounds.center;
            }
        }
    }
}