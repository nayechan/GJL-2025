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
        PlayerCombatStatus combatStatus = (PlayerCombatStatus) player.CombatStatus;
        combatStatus.OnExpChanged += Refresh;
    }

    private void Refresh()
    {
        PlayerCombatStatus combatStatus = (PlayerCombatStatus) player.CombatStatus;
        image.fillAmount = combatStatus.ExpRatio;
        levelText.text = $"Lv. {combatStatus.level:N0}";
    }
        
}