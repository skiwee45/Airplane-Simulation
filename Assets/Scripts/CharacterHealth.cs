using System;
using UnityEngine;

public class CharacterHealth : MonoBehaviour, IHaveHealth
{
    [field : SerializeField]
    public virtual float Health { get; private set; }
    [field : SerializeField]
    public virtual float MaxHealth { get; private set; }

    [field: SerializeField] 
    public virtual float HealthPercent { get; private set; } //value from 0-1

    private void Awake()
    {
        Health = MaxHealth;
        HealthPercent = 100;
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        HealthPercent = Health / MaxHealth;
        if (Health <= 0)
        {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}