using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGoldIndicator : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text goldText;
    
    // Start is called before the first frame update
    void Start()
    {
        player.PlayerStatus.OnGoldChanged.AddListener(()=>Refresh(player.PlayerStatus.Gold));
    }

    void Refresh(long gold)
    {
        goldText.text = $"{gold:N0}";
    }
}
