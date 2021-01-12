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

        private void OnCollisionStay(Collision collision)
        {
            var forceVariable = CalculateCrashForce(collision);
            var importanceVariable = CalculateImpactImportance(collision);
            var damage = forceVariable * importanceVariable / 100f * damageMultiplier; //importanceVariable is out of 100
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
                var thisCollide = thisContact.thisCollider;
                if (dict.ContainsKey(thisCollide))
                {
                    dict[thisCollide] += 1;
                } else
                {
                    dict.Add(thisCollide, 1);
                }
            }
        
            //calculated based on weighted average
            var total = 0f;
            foreach (var colliderWeight in dict)
            {
                var importance = _colliderManager.ExtendedColliderDictionary[colliderWeight.Key].config.importance;
                var weight = colliderWeight.Value;
                total += importance * weight;
            }
            var average = total / numberOfContacts;
            return average;
        }

        private float CalculateCrashForce(Collision collision)
        {
            var collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
            return collisionForce;
        }
    }
}
