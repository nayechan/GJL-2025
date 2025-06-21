using System;
using UnityEngine;

[Serializable]
public enum RewardGrade
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

[Serializable]
[CreateAssetMenu(fileName = "New Reward", menuName = "Add Reward")]
public class Reward : ScriptableObject
{
    public Sprite sprite;
    public Ability ability;
    public float amount;
    public RewardGrade rewardGrade;
    public string description;
}