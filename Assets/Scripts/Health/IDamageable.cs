﻿namespace Health
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void Destroy();
    }
}