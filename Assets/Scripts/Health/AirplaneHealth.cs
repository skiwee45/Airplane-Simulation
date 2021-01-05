using ColliderAddon;
using UnityEngine;

namespace Health
{
    [RequireComponent(typeof(CollisionDamage))]
    public class AirplaneHealth : CharacterHealth
    {
        private CollisionDamage _collisionDamage;

        private void Start()
        {
            _collisionDamage = GetComponent<CollisionDamage>();
            _collisionDamage.OnCollisionDamage += TakeDamage;
        }

        private void OnDisable()
        {
            _collisionDamage.OnCollisionDamage -= TakeDamage;
        }
    }
}