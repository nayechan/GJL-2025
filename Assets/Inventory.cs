using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private bool isSwitchCooldown = false;
    
    [SerializeField] private Player player;
    
    [SerializeField] private ScriptableObject[] inventorySlots;
    [SerializeField] private Image[] equipImages;

    [SerializeField] private float switchDelay = 1.0f;

    // Update is called once per frame
    void Update()
    {
        KeyCode[] keyCodes =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, 
            KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V
        };

        for (int i = 0; i < keyCodes.Length; ++i)
        {
            if(Input.GetKeyDown(keyCodes[i]))
                UseItemInSlot(i);
        } 
    }

    private void Start()
    {
        for (int slot = 0; slot < inventorySlots.Length; ++slot)
        {

            IEquippable equippable = (IEquippable)inventorySlots[slot];
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

    public void UseItemInSlot(int slot)
    {
        if (isSwitchCooldown) return;
        
        IEquippable equippable = (IEquippable)inventorySlots[slot];
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
