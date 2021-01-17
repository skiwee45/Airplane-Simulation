using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColliderAddon
{
    [RequireComponent(typeof(ColliderManager))]
    public class CollisionDamage : MonoBehaviour
    {
        [SerializeField]
        private float damageMultiplier;
        
        private ColliderManager _colliderManager;

        //event
        public delegate void OnCollision(float damage);
        public event OnCollision OnCollisionDamage;

        private void Start()
        {
            _colliderManager = GetComponent<ColliderManager>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var forceVariable = CalculateCrashForce(collision);
            var importanceVariable = CalculateImpactImportance(collision);
            var damage = forceVariable * importanceVariable / 100f * damageMultiplier; //importanceVariable is out of 100
            Debug.Log("\tForce: " + forceVariable + "\tImportance: " + importanceVariable + "\tDamage: " + damage);
            OnCollisionDamage?.Invoke(damage);
        }

        private float CalculateImpactImportance(Collision collision)
        {
            var contacts = new List<ContactPoint>(collision.contactCount);
            var numberOfContacts = collision.GetContacts(contacts);
        
            //calculate the how much it hit each collider
            var dict = new Dictionary<Collider, int>();
            foreach (var thisContact in contacts)
            {
                var thisCollider = thisContact.thisCollider;
                if (thisCollider == null)
                {
                    Debug.Log("Null" + thisContact.point);
                    continue;
                }
                Debug.Log("Name: " + thisCollider + "\tID" + thisCollider.GetInstanceID() + "\tPoint Of Contact: " + thisContact.point);
                if (dict.ContainsKey(thisCollider))
                {
                    dict[thisCollider] += 1;
                } else
                {
                    dict.Add(thisCollider, 1);
                }
            }
        
            //calculated based on weighted average
            var total = 0f;
            foreach (var colliderWeight in dict)
            {
                var importance = _colliderManager.ExtendedColliderDictionary[colliderWeight.Key].Importance;
                var weight = colliderWeight.Value;
                total += importance * weight;
            }

            var average = total / dict.Values.ToList().Sum();
            return average;
        }

        private float CalculateCrashForce(Collision collision)
        {
            var collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
            return collisionForce;
        }
    }
}
