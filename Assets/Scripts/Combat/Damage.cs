public class Damage
{
    public long Amount { get; }
    
    public float KnockbackDuration { get; }
    public float KnockbackIntensity { get; }
    public float ParalyzeDuration { get; }
    
    public bool IsCritical { get; }
    
    public IAttacker Source { get; }
    public DamageType Type { get; }

    public Damage(long amount, IAttacker source, DamageType type, bool isCritical,
        float knockbackDuration = 0f, float knockbackIntensity = 0f, float paralyzeDuration = 0f)
    {
        Amount = amount;
        Source = source;
        Type = type;
        IsCritical = isCritical;
        KnockbackDuration = knockbackDuration;
        KnockbackIntensity = knockbackIntensity;
        ParalyzeDuration = paralyzeDuration;
    }
}