using UnityEngine;

public enum LootType
{
    WEAPON,
    GEAR,
    HEALTH
}

public abstract class Item : ScriptableObject
{
    public string itemName;
    public LootType type;
}
