using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    
    private bool isSwitchCooldown = false;
    
    private Player player;
    
    [SerializeField] private ScriptableObject[] inventorySlots;
    [SerializeField] private Image[] equipImages;

    [SerializeField] private float switchDelay = 0.5f;

    private bool isInitialized = false;

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;
        
        KeyCode[] keyCodes =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3
        };

        for (int i = 0; i < keyCodes.Length; ++i)
        {
            if(Input.GetKeyDown(keyCodes[i]))
                UseItemInSlot(i);
        } 
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(()=>Player.Instance != null);
        isInitialized = true;
        player = Player.Instance;
        UseItemInSlot(0, true);
        
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

    public void UseItemInSlot(int slot, bool ignoreCooldown = false)
    {
        if (isSwitchCooldown) return;
        
        IEquippable equippable = (IEquippable)inventorySlots[slot];
        if (equippable != null)
        {
            equippable.OnUse(player);
        }

        if (!ignoreCooldown)
        {
            isSwitchCooldown = true;
            StartCoroutine(ResetSwitchCooldown());
        }
    }

    IEnumerator ResetSwitchCooldown()
    {
        yield return new WaitForSeconds(switchDelay);
        isSwitchCooldown = false;
    }
}
