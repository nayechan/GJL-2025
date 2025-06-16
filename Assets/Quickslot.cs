using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Quickslot : MonoBehaviour
{
    private bool isSwitchCooldown = false;
    
    [SerializeField] private Player player;
    
    [SerializeField] private ScriptableObject[] equipQuickslots;
    [SerializeField] private Image[] equipImages;

    [SerializeField] private float switchDelay = 1.0f;

    // Update is called once per frame
    void Update()
    {
        KeyCode[] keyCodes =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, 
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8
        };

        for (int i = 0; i < keyCodes.Length; ++i)
        {
            if(Input.GetKeyDown(keyCodes[i]))
                EquipQuickslot(i);
        } 
    }

    private void Start()
    {
        for (int slot = 0; slot < equipQuickslots.Length; ++slot)
        {

            IEquippable equippable = (IEquippable)equipQuickslots[slot];
            if (equippable != null)
            {
                equipImages[slot].sprite = equippable.Sprite;
                equipImages[slot].color = Color.white;
            }
            else
            {
                equipImages[slot].color = Color.clear;
            }
        }
    }

    void EquipQuickslot(int slot)
    {
        if (isSwitchCooldown) return;
        
        IEquippable equippable = (IEquippable)equipQuickslots[slot];
        if (equippable != null)
        {
            equippable.OnUse(player);
        }

        isSwitchCooldown = true;
        StartCoroutine(ResetSwitchCooldown());
    }

    IEnumerator ResetSwitchCooldown()
    {
        yield return new WaitForSeconds(switchDelay);
        isSwitchCooldown = false;
    }
}
