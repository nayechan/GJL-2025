using System.Linq;

namespace Combat.Modifier
{
    public class WeaponStatModifier : IStatModifier
    {
        public string WeaponType { get; private set; }
        public string ModifierType { get; private set; }
        public float Amount { get; private set; }
        public BackupModifier BackupField { get; set; }

        public WeaponStatModifier(string _weaponType, string _modifierType, float _amount, Ability _ability)
        {
            WeaponType = _weaponType;
            ModifierType = _modifierType;
            Amount = _amount;
            BackupField = new BackupModifier(_ability, _amount);
        }

        public static string ExtractWeaponType(string abilityType)
        {
            string[] prefixes = { "Sword", "Shuriken", "Wand" };
            return prefixes.First(prefix => abilityType.StartsWith(prefix));
        }

        public static string ExtractModifierType(string abilityType)
        {
            string[] suffixes = { "ATK", "CriticalRate", "CriticalDamage", "Size",
                "Cooldown", "Drain", "Count","Penetration", "MagicRadius" };
            return suffixes.First(suffix => abilityType.EndsWith(suffix));
        }
    }
}