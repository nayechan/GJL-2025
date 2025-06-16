using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
    public Sprite Sprite { get; }
    public int Count { get; }
    public void OnUse(Player player);
    
}
