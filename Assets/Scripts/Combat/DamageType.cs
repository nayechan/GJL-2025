public enum DamageType
{
    Physical,
    Magical,
    Ranged
}

public static class DamageUtility
{
    public static float GetEffectiveDamage(DamageType damageType)
    {
        float[] effectiveDamage = { 1.0f, 0.6f, 0.7f };
        return effectiveDamage[(int)damageType];
    }
}