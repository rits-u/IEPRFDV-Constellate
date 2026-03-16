using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ItemDisplay : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private TextMeshProUGUI nameTextBox;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image iconItem;
    [SerializeField] private TextMeshProUGUI tierTextBox;
    [SerializeField] private Transform slotsHolder;

    private List<TextMeshProUGUI> slotStats = new();
    private List<Image> icons = new();

    [Header("Icon Mappings")]
    private static Dictionary<InfoType, Sprite> iconDictionary;
    [SerializeField] private List<StatIconPair> iconMappings;

    [System.Serializable]
    public struct StatIconPair
    {
        public InfoType type;
        public Sprite sprite;
    }

    private void Awake()
    {
        iconDictionary = new Dictionary<InfoType, Sprite>();

        foreach (var pair in iconMappings)
        {
            if (!iconDictionary.ContainsKey(pair.type))
                iconDictionary.Add(pair.type, pair.sprite);
        }

        for(int i = 0; i < slotsHolder.childCount; i++)
        {
            var child = slotsHolder.GetChild(i);
            slotStats.Add(child.GetComponent<TextMeshProUGUI>());
            var grandChild = child.GetComponentInChildren<Image>();
            icons.Add(grandChild);
        }
    }

    public void SelectDisplay()
    {
        selectPanel.gameObject.SetActive(true);
    }

    public void UnselectDisplay()
    {
        selectPanel.gameObject.SetActive(false);
    }

    private void EditStatByIndex(string text, int index)
    {
        slotStats[index].text = text;
        slotStats[index].gameObject.SetActive(true);
        if (text == "0") slotStats[index].text = "";
    }

    private void EditNameTextBox(string name) { 
        nameTextBox.text = name;
    }

    private void EditTypeTextBox(string type)
    {
        itemType.text = type;
    }

    private void EditTierTextBox(string tier)
    {
        tierTextBox.text = tier;
    }

    //public void EditDescTextBox(string desc)
    //{
    //    descTextBox.text = desc;
    //}

    private void SetIconInfoByIndex(InfoType type, int index)
    {
        if (iconDictionary.TryGetValue(type, out Sprite sprite))
        {
            icons[index].sprite = sprite;
            icons[index].gameObject.SetActive(true);
        }
    }

    private void HideUnusedSlots()
    {
        foreach(var icon in icons)
        {
            if(icon.sprite == null) icon.gameObject.SetActive(false);
        }
    }

    private void SetIconItem(Sprite sprite)
    {
        iconItem.sprite = sprite;
    }


    public void Setup(Item item)
    {
        switch (item)
        {
            case Range range:
                DisplayRangeItem((Range)item);
                break;

            case Melee melee:
                DisplayMeleeItem((Melee)item);
                break;

            case Gear gear:
                DisplayGearItem((Gear)item);
                break;

            case Heal heal:
                DisplayHealItem((Heal)item);
                break;
        }
    }

    public void DisplayEquippedGear(Gear gear)
    {
        EditNameTextBox(gear.itemName);
        EditTypeTextBox("Gear");
        SetIconItem(gear.sprite);
        EditTierTextBox($"Tier {gear.CurrentTier}");

        Dictionary<InfoType, bool> stats = new();
        if (gear.hasHPStat) stats.Add(InfoType.HP, gear.hasHPStat);
        if (gear.hasATKStat) stats.Add(InfoType.ATK, gear.hasATKStat);
        if (gear.hasSPStat) stats.Add(InfoType.SP, gear.hasSPStat);

        int index = 0;
        foreach (var stat in stats)
        {
            string text = gear.GetStatsByType(stat.Key).ToString();
            EditStatByIndex(text, index);
            SetIconInfoByIndex(stat.Key, index);
            index++;
        }

        EditStatByIndex(gear.Expiration.ToString(), index);
        SetIconInfoByIndex(InfoType.EXPIRATION, index);
        HideUnusedSlots();
    }

    private void DisplayRangeItem(Range range)
    {
        EditNameTextBox(range.itemName);
        EditTypeTextBox("Weapon");
        SetIconItem(range.sprite);

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
            EditStatByIndex(text, index);
            SetIconInfoByIndex(type, index);
            index++;
        }

        HideUnusedSlots();
    }



    private void DisplayMeleeItem(Melee melee)
    {
        EditNameTextBox(melee.itemName);
        EditTypeTextBox("Weapon");
        SetIconItem(melee.sprite);

        int index = 0;
        foreach (InfoType type in Enum.GetValues(typeof(InfoType)))
        {
            if (type != InfoType.DAMAGE && type != InfoType.SLASH_INTERVAL) continue;

            int T1 = (int)melee.GetProperty(type, 1);
            int T2 = (int)melee.GetProperty(type, 2);
            string text;
            if (T1 != T2) text = $"{T1} <color=green>→{T2}</color>";
            else text = $"{T1}";
            EditStatByIndex(text, index);
            SetIconInfoByIndex(type, index);
            index++;
        }

        HideUnusedSlots();
    }

    private void DisplayGearItem(Gear gear)
    {
        EditNameTextBox(gear.itemName);
        EditTypeTextBox("Gear");
        SetIconItem(gear.sprite);

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

            EditStatByIndex(text, index);
            SetIconInfoByIndex(stat.Key, index);
            index++;
        }

        EditStatByIndex(gear.Expiration.ToString(), index);
        SetIconInfoByIndex(InfoType.EXPIRATION, index);
        HideUnusedSlots();
    }

    private void DisplayHealItem(Heal heal)
    {
        EditNameTextBox(heal.itemName);

        EditStatByIndex(heal.HealAmount.ToString(), 0);
        SetIconInfoByIndex(InfoType.HEAL, 0);
        HideUnusedSlots();
    }

}
