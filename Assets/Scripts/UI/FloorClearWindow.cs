using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloorClearWindow : Window
{
    [SerializeField] private GameObject choicePrefab, rerollButtonPrefab;
    [SerializeField] private AudioClip cardReceiveSound;
    
    [SerializeField] private TMP_Text titleTextField;
    [SerializeField] private Transform choiceTransform;
    
    [SerializeField] private Reward[] rewards;

    private List<Reward> currentRewards;

    private Dictionary<RewardGrade, List<Reward>> rewardsByGrade;

    public RewardGrade RollRewardGrade(float floor)
    {
        // floor: 1~25
        float common    = Mathf.Clamp01(0.25f - 0.012f * (floor - 1)); // 0.25 → 0
        float uncommon  = Mathf.Clamp01(0.25f - 0.01f * (floor - 1)); // 0.25 → 0
        float rare      = Mathf.Clamp01(0.25f - 0.005f * Mathf.Abs(floor - 13)); // 피크: 13층
        float epic      = Mathf.Clamp01(0.1f  + 0.01f * (floor - 1));  // 0.1 → 0.34
        float legendary = Mathf.Clamp01(0.03f + 0.015f * (floor - 1)); // 0.03 → 0.39
        float mythic    = Mathf.Clamp01(0.01f + 0.008f * (floor - 1)); // 0.01 → 0.21


        // 정규화 (합이 1이 되도록)
        float total = common + uncommon + rare + epic + legendary + mythic;

        common    /= total;
        uncommon  /= total;
        rare      /= total;
        epic      /= total;
        legendary /= total;
        mythic    /= total;

        float roll = UnityEngine.Random.value;
        float cumulative = 0f;

        if ((cumulative += common) > roll) return RewardGrade.Common;
        if ((cumulative += uncommon) > roll) return RewardGrade.Uncommon;
        if ((cumulative += rare) > roll) return RewardGrade.Rare;
        if ((cumulative += epic) > roll) return RewardGrade.Epic;
        if ((cumulative += legendary) > roll) return RewardGrade.Legendary;
        return RewardGrade.Mythic;
    }
    
    private void OnEnable()
    {
        Refresh();
    }

    private void Awake()
    {
        currentRewards = new List<Reward>();
        rewardsByGrade = new Dictionary<RewardGrade, List<Reward>>();
        rewardsByGrade = rewards.GroupBy(x => x.rewardGrade)
            .ToDictionary(x => x.Key, x => x.ToList());

    }
    
    private void Refresh()
    {
        int floor = StageManager.Instance.CurrentFloor;
        titleTextField.text = $"{floor}F Clear!!";
        RerollReward();
    }

    public void RerollReward()
    {
        int floor = StageManager.Instance.CurrentFloor;
        float lukBonus = Player.Instance.GetStat(BaseStatType.LUK) / 10.0f;

        foreach (Transform child in choiceTransform)
        {
            Destroy(child.gameObject);
        }
        
        currentRewards.Clear();
        for (int i = 0; i < 3; ++i)
        {
            RewardGrade rewardGrade = RollRewardGrade(floor + lukBonus);
            int rewardCount = rewardsByGrade[rewardGrade].Count;
            Reward reward = rewardsByGrade[rewardGrade][UnityEngine.Random.Range(0, rewardCount)];
            currentRewards.Add(reward);
        }

        for (int i = 0; i < 3; ++i)
        {
            Vector2 pos = new Vector2(600 * (i - 1), 64);
            GameObject choiceGameObj = Instantiate(choicePrefab, choiceTransform);
            choiceGameObj.GetComponent<RectTransform>().anchoredPosition = pos;
            choiceGameObj.GetComponent<RewardChoice>().Init(currentRewards[i], 
                _reward => OnConfirmReward(_reward));
        }
    }

    public void OnConfirmReward(Reward _reward)
    {
        Player.Instance.ReceiveReward(_reward);
        AudioManager.Instance.PlaySFX(cardReceiveSound);
        Close();
    }
}
