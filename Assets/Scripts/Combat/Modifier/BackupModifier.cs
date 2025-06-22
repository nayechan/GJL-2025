using System;

[Serializable]
public struct BackupModifier
{
    public Ability ability;
    public float amount;

    public BackupModifier(Ability _ability, float _amount)
    {
        ability = _ability;
        amount = _amount;
    }
}