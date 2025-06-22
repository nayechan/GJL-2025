using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerStatus
{
    private Player player;
    
    [field: SerializeField]
    public UnityEvent OnExpChanged { get; private set; }
    
    [field: SerializeField]
    public UnityEvent OnGoldChanged { get; private set; }
    
    [field: SerializeField]
    public UnityEvent OnLevelUp { get; private set; }

    private long ap = 0;
    public long Ap
    {
        get => ap;
        set
        {
            ap = value;
            player.CombatStatus.RefreshStat();
        } 
    }
    
    private long currentExp = 0;
    public long CurrentExp
    {
        get => currentExp;
        private set
        {
            currentExp = value;
            OnExpChanged?.Invoke();
        }
    }

    private long gold = 0;

    public long Gold
    {
        get => gold;
        private set
        {
            gold = value;
            OnGoldChanged?.Invoke();
        }
    }


    public long MaxExp => (long)(Mathf.Pow(player.CombatStatus.level, 1.3f) * 30 + 20);
    
    /// <summary>
    /// 경험치를 얻고, currentExp > MaxExp일때 레벨업합니다
    /// </summary>
    public void GainExp(long amount)
    {
        CurrentExp += amount;
        if (CurrentExp >= MaxExp)
        {
            LevelUp();
        }
    }

    public void GainGold(long amount)
    {
        Gold += amount;
    }

    public void LevelUp()
    {
        while (currentExp >= MaxExp)
        {
            currentExp -= MaxExp;
            ++player.CombatStatus.level;
        }
        Ap += GetAp(Level);
        OnLevelUp?.Invoke();
        player.CombatStatus.CurrentHp = player.GetMaxHp();
    }

    public long GetAp(long level)
    {
        if (level < 10)
            return 1;
        if (level < 30)
            return 2;
        return 3;
    }       
    
    public long Level => player.CombatStatus.level;

    public float ExpRatio => MaxExp > 0 ? (float)currentExp / MaxExp : 0f;
    
    public void Init(Player _player)
    {
        player = _player;
        
        OnLevelUp.AddListener(()=>OnExpChanged?.Invoke());
    }

}