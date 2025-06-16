using UnityEngine;

public abstract class Consumable : MonoBehaviour, IEquippable
{
    public virtual Sprite Sprite { get; }
    public virtual int Count { get; }
    public abstract void OnUse(Player player);
}