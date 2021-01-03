using System.Collections.Generic;
using ColliderAddon;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// This class is in order to calculate forces of a collision and calculate damage on the plane
    /// </summary>
    public class AirplaneHealth : CharacterHealth
    {
        //health fields all done in CharacterHealth
        private void OnCollisionEnter(Collision collision)
        {
            var forceVariable = CalculateCrashForce(collision);
            var impactedPartVariable = CalculateImpactImportance(collision);
        }

        private float CalculateImpactImportance(Collision collision)
        {
            var contacts = new List<ContactPoint>(collision.contactCount);
            var numberOfContacts = collision.GetContacts(contacts);
        
            //calculate the how much it hit each collider
            var dict = new Dictionary<Collider, int>();
            foreach (var contact in contacts)
            {
                var thisCollider = contact.thisCollider;
                if (dict.ContainsKey(thisCollider))
                {
                    dict[thisCollider] += 1;
                }
                else
                {
                    dict.Add(thisCollider, 1);
                }
            }
        
            //calculated based on weighted average
            var total = 0f;
            var colliderManager = GetComponent<ColliderManager>();
            if (colliderManager != null)
            {
                foreach (var collider in dict)
                {
                    //total += colliderManager.Colliders[collider.Key].importance;
                }
            }
            else
            {
                Debug.Log("No colliderManager found on gameobject");
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