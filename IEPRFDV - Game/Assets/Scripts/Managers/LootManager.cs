using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    [SerializeField] private List<Item> lootDrops = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this.gameObject);
    }

    public void AddItemToLoot(Item item)
    {
        lootDrops.Add(item);
       // Debug.Log($"LM: loot count: {lootDrops.Count}");
    }

    public void RemoveItemFromLoot(Item item)
    {
        lootDrops.Remove(item);
    }

    public void ResolveLoot(QTEResult result)
    {
        switch (result)
        {
            case QTEResult.Share:
                DistributeItem(1, 1);
                DistributeItem(2, 1);
                Debug.Log($"Each player gets Tier 1");
                break;
            case QTEResult.P1Steals:
                DistributeItem(1, 2);
                Debug.Log($"P1 gets Tier 2, P2 gets none");
                break;
            case QTEResult.P2Steals:
                DistributeItem(2, 2);
                Debug.Log($"P1 gets none, P2 gets Tier 2");
                break;
            case QTEResult.None:
                Debug.Log($"get good");
                break;
        }

        lootDrops.Clear();
    }

    private void DistributeItem(int playerID, int tier)
    {
        int gearCount = 0;

        //DISCARD
        foreach(Item item in lootDrops)
        {
            if (item.type == ItemType.GEAR) gearCount++;
        }

        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        int equipped = inventory.GetEquippedGearCount();
        if (inventory.GetMaxSlots() <= equipped + gearCount)
        {
            Debug.Log("its full");
        }


        foreach (Item item in lootDrops)
        {
            if (item.type == ItemType.GEAR)
            {
                Gear gear = (Gear)item;
                //Discard system
              //  PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
                //check




                PlayerManager.Instance.ApplyGearToPlayer(gear, playerID, tier);
            }
            else if (item.type == ItemType.WEAPON)
            {
                Weapon weapon = (Weapon)item;
                PlayerManager.Instance.SwitchWeaponOfPlayer(weapon, playerID, tier);
            }
            else if(item.type == ItemType.HEAL)
            {
                Heal heal = (Heal)item;
                PlayerManager.Instance.ApplyHealToPlayer(heal, playerID, tier);
            }

        }

        
    }
}
