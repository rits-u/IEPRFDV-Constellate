using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    [SerializeField] private List<Item> lootDrops = new();

    public QTEResult result;

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
    
    public void ResolveLoot()
    {
        StartCoroutine(ResolveLootRoutine(result));
    }


    //need to update
    private IEnumerator ResolveLootRoutine(QTEResult result)
    {
        yield return new WaitForSeconds(1f);
        List<Item> lootCopy = new List<Item>(lootDrops);
        switch(result)
        {
            case QTEResult.Share:
                yield return StartCoroutine(DistributeItem(1, 1, lootCopy));
                break;
        }
    }

    private IEnumerator DistributeItem(int playerID, int tier, List<Item> loot)
    {
        int gearCount = 0;
        List<Gear> gearLoots = new();

        foreach (Item item in loot)
        {
            if (item.type == ItemType.GEAR)
            {
                gearCount++;
                gearLoots.Add((Gear)item);
            }
        }

        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        int equipped = inventory.GetEquippedGearCount();

        int overflow = (equipped + gearCount) - inventory.GetMaxSlots();

        if (overflow > 0)
        {
            Debug.Log("inventory is full, will need to discard");
            bool discardFinished = false;

            DiscardUI ui = (DiscardUI)UIManager.Instance.GetScreen("Discard");
            ui.InitializeDisplayPanel(gearLoots);

            UIManager.Instance.OpenScreen("Discard");
            ui.OpenDiscardWindow(playerID, overflow, tier, () => { discardFinished = true; });

            yield return new WaitUntil(() => discardFinished);
        }
        else
        {
            foreach (Item item in loot)
            {
                switch (item)
                {
                    case Gear gear:
                        PlayerManager.Instance.ApplyGearToPlayer(gear, playerID, tier);
                        break;
                    case Weapon weapon:
                        PlayerManager.Instance.SwitchWeaponOfPlayer(weapon, playerID, tier);
                        break;
                    case Heal heal:
                        PlayerManager.Instance.ApplyHealToPlayer(heal, playerID, tier);
                        break;
                }

                RemoveItemFromLoot(item);
            }
        }

        lootDrops.Clear();
        gearLoots.Clear();

        RoundManager.Instance.NextRound();
    }
}
