using System;

public class BaseStatModifier : IStatModifier
{
    public BaseStatType StatType { get; protected set; }
    public float Amount { get; protected set; }
    public BackupModifier BackupField { get; set; }

    public BaseStatModifier(string _statType, float _amount, Ability _ability)
    {
        Enum.TryParse(_statType, true, out BaseStatType statType);
        StatType = statType;
        Amount = _amount;
        BackupField = new BackupModifier(_ability, _amount);
    }
}