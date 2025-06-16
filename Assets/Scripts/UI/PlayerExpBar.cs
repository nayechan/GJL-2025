using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text levelText;
    private void Start()
    {
        PlayerStatus playerStatus = player.PlayerStatus;
        playerStatus.OnExpChanged += Refresh;
    }

    private void Refresh()
    {
        PlayerStatus playerStatus = player.PlayerStatus;
        image.fillAmount = playerStatus.ExpRatio;
        levelText.text = $"Lv. {playerStatus.Level:N0}";
    }
        
}