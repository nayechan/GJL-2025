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
        player.CombatStatus.OnExpChanged += Refresh;
    }

    private void Refresh()
    {
        image.fillAmount = player.CombatStatus.ExpRatio;
        levelText.text = $"Lv. {player.CombatStatus.level:N0}";
    }
        
}