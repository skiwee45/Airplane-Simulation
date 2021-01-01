using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is in order to calculate forces of a collision and calculate damage on the plane
/// </summary>
public class AirplaneHealth : CharacterHealth
{
    //health fields all done in CharacterHealth
    private void OnCollisionEnter(Collision collision)
    {
        var forceVariable = GetCrashForce(collision);
        var impactedPartVariable = GetImpactPartDamage(collision);
    }

    private float GetImpactPartDamage(Collision collision)
    {
        var contacts = new List<ContactPoint>(collision.contactCount);
        var numberOfContacts = collision.GetContacts(contacts);
        
        //find out which collider it hit most
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
        
        //
    }

    private float GetCrashForce(Collision collision)
    {
        var collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        return collisionForce;
    }
}