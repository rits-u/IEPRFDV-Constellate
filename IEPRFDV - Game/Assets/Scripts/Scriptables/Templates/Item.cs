using UnityEngine;

public enum LootType
{
    WEAPON,
    GEAR,
    HEAL
}

public abstract class Item : ScriptableObject
{
    public string itemName;
    public LootType type;

   // public virtual void UpgradeToNextTier() { }
}
