using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;

    [SerializeField] private List<Item> lootDrops = new();

    public QTEResult result;
    private int toDistribute = 0;
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

    private IEnumerator ResolveLootRoutine(QTEResult result)
    {
        yield return new WaitForSeconds(1f);
      
        switch (result)
        {
            case QTEResult.Share:
                toDistribute = 2;
                yield return StartCoroutine(DistributeItem(1, 1));
                yield return StartCoroutine(DistributeItem(2, 1));
                break;

            case QTEResult.P1Steals:
                toDistribute = 1;
                yield return StartCoroutine(DistributeItem(1, 2));
                break;

            case QTEResult.P2Steals:
                toDistribute = 1;
                yield return StartCoroutine(DistributeItem(2, 2));
                break;

            case QTEResult.None:
                RoundManager.Instance.NextRound();
                break;

        }
    }

    private IEnumerator DistributeItem(int playerID, int tier)
    {
        int gearCount = 0;
        List<Item> lootCopy = new List<Item>(lootDrops);
        List<Gear> gearLoots = new();

        foreach (Item item in lootCopy)
        {
            if (item.type == ItemType.GEAR)
            {
                gearCount++;
                gearLoots.Add((Gear)item);
            }
        }

        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        int equipped = inventory.GetEquippedGearCount();
        int overflow = (equipped + gearCount) - inventory.GetMaxSlots(); //check if theres overflow

        //discard
        if (overflow > 0)
        {
            //Debug.Log("inventory is full, will need to discard");
            List<Gear> remainingGear = null;
            bool discardFinished = false;

            DiscardUI ui = (DiscardUI)UIManager.Instance.GetScreen("Discard");
            ui.Initialize(gearLoots);

            UIManager.Instance.OpenScreen("Discard");
            ui.OpenDiscardWindow(playerID, tier, (result) =>
            {
                remainingGear = result;
                discardFinished = true;
            });

            yield return new WaitUntil(() => discardFinished);

            lootCopy.RemoveAll(item =>
                item is Gear gear && !remainingGear.Contains(gear));


        }


        foreach (Item item in lootCopy)
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
        }

        lootCopy.Clear();
        gearLoots.Clear();


        toDistribute--;
        Debug.Log("to distribute: " + toDistribute);

        if (toDistribute == 0) {
            lootDrops.Clear();
            RoundManager.Instance.NextRound();
        }
    }
}
