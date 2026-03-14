using UnityEngine;

public enum ItemType
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
    PROJECTILES,
    HEAL,
    BURST_INTERVAL,
    FIRE_RATE, //for guns interval
    SLASH_INTERVAL,
    EXPIRATION
}

public abstract class Item : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public ItemType type;
    public Sprite sprite;
}
