using System.Collections.Generic;
using System;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    [Header("Loot Properties")]
    [SerializeField] private int maxLoot = 3;
    [SerializeField] private WeightedRandomList<Item> lootTable;

    [Header("UI Elements")]
    [SerializeField] private GameObject upperPanel;
    [SerializeField] private GameObject displayPrefab;

    private List<GameObject> itemDisplays = new();

    public void RandomizeLoot()
    {
        int numLoot = UnityEngine.Random.Range(1, maxLoot+1);
        int lootWeaponAmount = 0;
        int lootHeal = 0;
        ResetDisplay();

        for (int i = 0; i < numLoot; i++)
        {
            var item = lootTable.GetRandom();

            //check if it gets weapon/heal again
            if((item.type == ItemType.WEAPON && lootWeaponAmount != 0) ||
                item.type == ItemType.HEAL && lootHeal != 0)
            {
                i--;
                continue;
            }

            switch (item.type)
            {
                case ItemType.WEAPON:
                    Weapon weapon = (Weapon)item.item;
                    if (weapon.IsMelee) DisplayMeleeItem((Melee)weapon);
                    else DisplayRangeItem((Range)weapon);
 
                    lootWeaponAmount++;
                    lootTable.Remove(item); //remove from the drops
                    LootManager.Instance.AddItemToLoot(item.item);

                    break;
                
                case ItemType.GEAR:
                    Gear gear = (Gear)item.item;
                    DisplayGearItem(gear);
                    LootManager.Instance.AddItemToLoot(item.item);
                    break;

                case ItemType.HEAL:
                    Heal heal = (Heal)item.item;
                    DisplayHeal(heal);
                    LootManager.Instance.AddItemToLoot(item.item);
                    lootHeal++;
                    break;
            }
        }
    }

    private GameObject InstantiateDisplay()
    {
        GameObject display = Instantiate(displayPrefab);
        display.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        display.transform.SetParent(upperPanel.transform, false);
        display.SetActive(true);
        itemDisplays.Add(display);
        return display;
    }

    private void DisplayRangeItem(Range range)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(range.itemName);
        itemDisplay.EditTypeTextBox("Weapon");
        itemDisplay.SetIconItem(range.sprite);

        int index = 0;
        foreach (InfoType type in Enum.GetValues(typeof(InfoType)))
        {
            if (type != InfoType.DAMAGE && type != InfoType.PROJECTILES &&
                type != InfoType.FIRE_RATE) continue;

            int T1 = (int)range.GetProperty(type, 1);
            int T2 = (int)range.GetProperty(type, 2);
            string text;
            if (T1 != T2) text = $"{T1} <color=green>→{T2}</color>";
            else text = $"{T1}";
            itemDisplay.EditStatByIndex(text, index);
            itemDisplay.SetIconInfoByIndex(type, index);
            index++;
        }

        itemDisplay.HideUnusedSlots();
    }

    private void DisplayMeleeItem(Melee melee)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(melee.itemName);
        itemDisplay.EditTypeTextBox("Weapon");
        itemDisplay.SetIconItem(melee.sprite);

        int index = 0;
        foreach (InfoType type in Enum.GetValues(typeof(InfoType)))
        {
            if (type != InfoType.DAMAGE && type != InfoType.SLASH_INTERVAL) continue;

            int T1 = (int)melee.GetProperty(type, 1);
            int T2 = (int)melee.GetProperty(type, 2);
            string text;
            if (T1 != T2) text = $"{T1} <color=green>→{T2}</color>";
            else text = $"{T1}";
            itemDisplay.EditStatByIndex(text, index);
            itemDisplay.SetIconInfoByIndex(type, index);
            index++;
        }

        itemDisplay.HideUnusedSlots();
    }

    private void DisplayGearItem(Gear gear)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(gear.itemName);
        itemDisplay.EditTypeTextBox("Gear");
        itemDisplay.SetIconItem(gear.sprite);

        Dictionary<InfoType, bool> stats = new();
        if (gear.hasHPStat) stats.Add(InfoType.HP, gear.hasHPStat);
        if (gear.hasATKStat) stats.Add(InfoType.ATK, gear.hasATKStat);
        if (gear.hasSPStat) stats.Add(InfoType.SP, gear.hasSPStat);

        int index = 0;
        foreach (var stat in stats)
        {
            string text;
            int T1 = gear.GetStats(stat.Key, 1);
            int T2 = gear.GetStats(stat.Key, 2);

            if (T1 != T2) text = $"{T1} <color=green>→{T2}</color>";
            else text = $"{T1}";

            itemDisplay.EditStatByIndex(text, index);
            itemDisplay.SetIconInfoByIndex(stat.Key, index);
            index++;
        }

        itemDisplay.EditStatByIndex(gear.Expiration.ToString(), index);
        itemDisplay.SetIconInfoByIndex(InfoType.EXPIRATION, index);
        itemDisplay.HideUnusedSlots();
    }

    private void DisplayHeal(Heal heal)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(heal.itemName);

        itemDisplay.EditStatByIndex(heal.HealAmount.ToString(), 0);
        itemDisplay.SetIconInfoByIndex(InfoType.HEAL, 0);
        itemDisplay.HideUnusedSlots();
    }

    private void ResetDisplay()
    {
        foreach(var display in itemDisplays)
        {
            Destroy(display);
        }

        itemDisplays.Clear();
    }
}
