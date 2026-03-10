using UnityEngine;

public enum LootType
{
    WEAPON,
    GEAR,
    HEAL
}

public enum InfoType
{
    HP,
    ATK,
    SP,
    DAMAGE,
    BULLETS,
    HEAL,
    BURST_INTERVAL,
    FIRE_RATE, //for weapons interval
}

public abstract class Item : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public LootType type;

   // public virtual void UpgradeToNextTier() { }
}
