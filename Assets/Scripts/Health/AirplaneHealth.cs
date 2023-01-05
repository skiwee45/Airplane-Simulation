using System;
using ColliderAddon;
using UnityEngine;

namespace Health
{
    [RequireComponent(typeof(CollisionDamage))]
    public class AirplaneHealth : CharacterHealth
    {
        private CollisionDamage _collisionDamage;

        private new void Awake()
        {
            base.Awake();
            _collisionDamage = GetComponent<CollisionDamage>();
        }

        private void OnEnable()
        {
            _collisionDamage.OnCollisionDamage += TakeDamage;
        }

        private void OnDisable()
        {
            _collisionDamage.OnCollisionDamage -= TakeDamage;
        }
    }
}