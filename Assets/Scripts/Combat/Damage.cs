public class Damage
{
    public long Amount { get; }
    public IAttacker Source { get; }
    public DamageType Type { get; }

    public Damage(long amount, IAttacker source, DamageType type)
    {
        Amount = amount;
        Source = source;
        Type = type;
    }
}