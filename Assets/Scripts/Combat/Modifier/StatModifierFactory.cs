using Combat.Modifier;

public class StatModifierFactory
{
    public static IStatModifier CreateModifier(BackupModifier backupModifier)
    {
        return CreateModifier(backupModifier.ability, backupModifier.amount);
    }
    
    public static IStatModifier CreateModifier(Ability ability, float amount)
    {
        string abilityString = ability.ToString();
        
        switch (ability)
        {
            case Ability.STR:
            case Ability.DEX:
            case Ability.INT:
            case Ability.VIT:
            case Ability.AGI:
            case Ability.LUK:
                return new BaseStatModifier(abilityString, amount, ability);
            case Ability.SwordATK:
            case Ability.SwordCriticalRate:
            case Ability.SwordCriticalDamage:
            case Ability.SwordSize:
            case Ability.SwordCooldown:
            case Ability.SwordDrain:
            case Ability.ShurikenATK:
            case Ability.ShurikenCriticalRate:
            case Ability.ShurikenCriticalDamage:
            case Ability.ShurikenCount:
            case Ability.ShurikenPenetration:
            case Ability.ShurikenDrain:
            case Ability.WandATK:
            case Ability.WandCriticalRate:
            case Ability.WandCriticalDamage:
            case Ability.WandMagicRadius:
            case Ability.WandMagicCooldown:
            case Ability.WandDrain:
                return new WeaponStatModifier(
                    WeaponStatModifier.ExtractWeaponType(abilityString),
                    WeaponStatModifier.ExtractModifierType(abilityString),
                    amount, ability);
            default:
                return new ExtraStatModifier(abilityString, amount, ability);
        }
    }
}