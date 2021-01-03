using UnityEngine;

namespace ColliderAddon
{
    public static class ColliderUtil
    {
        public static float GetColliderVolumePercent(Collider collider)
        {
            var colliderVolume = GetColliderVolume(collider);
            var totalVolume = GetRigidBodyVolume(collider.attachedRigidbody);
            var percentage = colliderVolume / totalVolume * 100;
            return percentage;
        }

        public static float GetRigidBodyVolume(Rigidbody rb)
        {
            var originalMass = rb.mass;
            rb.SetDensity(1f);
            var volume = rb.mass;
            rb.mass = originalMass;
            return volume;
        }

        public static Vector3 GetColliderCenter(Collider collider)
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
    
        public static float GetColliderVolume(Collider collider)
        {
            float volume;
            switch (collider)
            {
                case BoxCollider boxCollider:
                    var boxDimensions = boxCollider.size;
                    Debug.Log("Box Size " + boxDimensions);
                    volume = boxDimensions.x * boxDimensions.y * boxDimensions.z;
                    break;
                case CapsuleCollider capsuleCollider:
                    var capsuleRadius = capsuleCollider.radius;
                    var capsuleLength = capsuleCollider.height - (capsuleRadius * 2);
                    volume = Mathf.PI * Mathf.Pow(capsuleRadius, 2) * ((4f / 3f) * capsuleRadius + capsuleLength);
                    break;
                case SphereCollider sphereCollider:
                    var sphereRadius = sphereCollider.radius;
                    volume = (4f/3f) * Mathf.PI * Mathf.Pow(sphereRadius, 3);
                    break;
                case WheelCollider wheelCollider:
                    volume = wheelCollider.mass;
                    break;
                default:
                    var colliderDimensions = collider.bounds.size;
                    volume = colliderDimensions.x * colliderDimensions.y * colliderDimensions.z;
                    break;
            }
            return volume;
        }
    }
}