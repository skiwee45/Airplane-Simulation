using UnityEditor.MemoryProfiler;
using UnityEngine.UIElements;

public interface IHaveHealth : IDamageable
{
    float MaxHealth { get;}
    float Health { get;}
    float HealthPercent { get; }
}