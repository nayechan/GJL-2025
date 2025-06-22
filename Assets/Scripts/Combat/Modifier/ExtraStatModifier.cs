public class ExtraStatModifier : IStatModifier
{
    public string StatType { get; }
    public float Amount { get; }
    public BackupModifier BackupField { get; set; }

    public ExtraStatModifier(string _statType, float _amount, Ability _ability)
    {
        StatType = _statType;
        Amount = _amount;
        BackupField = new BackupModifier(_ability, _amount);
    }
}