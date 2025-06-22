using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RewardChoice : MonoBehaviour
{
    [SerializeField] private Sprite[] cardSpriteByGrade = new Sprite[6];
    [SerializeField] private Image cardImage, itemImage;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text itemTitle, itemGrade, description;

    public void Init(Reward reward, Action<Reward> onConfirm)
    {
        cardImage.sprite = cardSpriteByGrade[(int)reward.rewardGrade];
        itemImage.sprite = reward.sprite;
        itemTitle.text = reward.name;
        itemGrade.text = reward.rewardGrade.ToString();
        description.text = reward.description;
        
        confirmButton.onClick.AddListener(()=>onConfirm?.Invoke(reward));
    }
}