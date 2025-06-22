using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGoldIndicator : MonoBehaviour
{
    private Player player;
    [SerializeField] private TMP_Text goldText;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        player = Player.Instance;
        Refresh(player.PlayerStatus.Gold); 
        player.PlayerStatus.OnGoldChanged.AddListener(OnGoldChanged);
    }

    void Refresh(long gold)
    {
        goldText.text = $"Score: {gold:N0}";
    }
    
    public void OnGoldChanged() => Refresh(player.PlayerStatus.Gold);
    
    private void OnDestroy()
    {
        if (player != null)
            player.PlayerStatus.OnGoldChanged.RemoveListener(OnGoldChanged);
    }

}
