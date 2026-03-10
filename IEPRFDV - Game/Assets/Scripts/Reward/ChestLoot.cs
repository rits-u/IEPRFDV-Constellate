using NUnit.Framework;
using System.Collections.Generic;
using System;
using Unity.XR.Oculus.Input;
using UnityEditor.Rendering;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    [Header("Loot Properties")]
    [SerializeField] private int maxLoot = 2;
    [SerializeField] private WeightedRandomList<Item> lootTable;

    [Header("UI Elements")]
    [SerializeField] private GameObject upperPanel;
    [SerializeField] private GameObject displayPrefab;

    private int lootWeaponAmount;

    public void RandomizeLoot()
    {
        int numLoot = UnityEngine.Random.Range(1, maxLoot);
      //  int numLoot = 3;
        lootWeaponAmount = 0;

        for (int i = 0; i < numLoot; i++)
        {
            var item = lootTable.GetRandom();

            //check if it gets weapon again
            if(item.type == LootType.WEAPON && lootWeaponAmount != 0)
            {
                i--;
                continue;
            }

            switch (item.type)
            {
                case LootType.WEAPON:
                    Gun gun = (Gun)item.item;
                    DisplayGunItem(gun);
                    lootWeaponAmount++;
                    lootTable.Remove(item); //remove from the drops
                    LootManager.Instance.AddItemToLoot(item.item);
                    break;
                
                case LootType.GEAR:
                    Gear gear = (Gear)item.item;
                    DisplayGearItem(gear);
                    LootManager.Instance.AddItemToLoot(item.item);
                    break;
                
                case LootType.HEAL:
                    Heal heal = (Heal)item.item;
                    DisplayHeal(heal);
                    LootManager.Instance.AddItemToLoot(item.item);
                    break;
            }            
        }
    }

    private GameObject InstantiateDisplay()
    {
        GameObject display = Instantiate(displayPrefab);
        display.transform.localScale = Vector3.one;
        display.transform.SetParent(upperPanel.transform, false);
        display.SetActive(true);

        return display;
    }

    private void DisplayGunItem(Gun gun)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(gun.itemName);

        int index = 0;
        foreach (InfoType type in Enum.GetValues(typeof(InfoType)))
        {
            if (type != InfoType.DAMAGE && type != InfoType.BULLETS && type != InfoType.FIRE_RATE) continue;

            int T1 = (int)gun.GetProperty(type, 1);
            int T2 = (int)gun.GetProperty(type, 2);
            string text;
            if (T1 != T2) text = $"{T1} <color=green>+{T2 - T1}</color>";
            else text = $"{T1}";
            itemDisplay.EditTextBoxByIndex(text, index);
            itemDisplay.SetIconByIndex(type, index);
            index++;
        }
    }

    private void DisplayGearItem(Gear gear)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(gear.itemName);

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

            text = $"{T1} <color=green>+{T2 - T1}</color>";

            itemDisplay.EditTextBoxByIndex(text, index);
            itemDisplay.SetIconByIndex(stat.Key, index);
            index++;
        }
    }

    private void DisplayHeal(Heal heal)
    {
        ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
        itemDisplay.EditNameTextBox(heal.itemName);

        itemDisplay.EditTextBoxByIndex(heal.HealAmount.ToString(), 0);
        itemDisplay.SetIconByIndex(InfoType.HEAL, 0);
    }
}
