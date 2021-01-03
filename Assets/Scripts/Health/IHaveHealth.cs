namespace Health
{
    public interface IHaveHealth : IDamageable
    {
        float MaxHealth { get;}
        float Health { get;}
        float HealthPercent { get; }
    }
}