using UnityEngine;

namespace Health
{
    public class CharacterHealth : MonoBehaviour, IHaveHealth
    {
        [SerializeField] 
        private float health;
        public virtual float Health => health;
        
        [SerializeField] 
        private float maxHealth;
        public virtual float MaxHealth => maxHealth;
        
        [SerializeField]
        [Range(0, 1)]
        private float healthPercent;
        public virtual float HealthPercent => healthPercent;

        protected void Awake()
        {
            health = maxHealth;
            healthPercent = 100;
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            healthPercent = health / maxHealth;
            if (health <= 0)
            {
                Destroy();
            }
        }

        public virtual void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}