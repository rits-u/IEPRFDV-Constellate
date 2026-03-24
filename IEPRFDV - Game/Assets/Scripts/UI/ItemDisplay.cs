using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void SetupHeader(string name, string type, Sprite icon)
    {   
        EditNameTextBox(name);
        EditTypeTextBox(type);
        SetIconItem(icon);
    }

    //public void EditDescTextBox(string desc)
    //{
    //    descTextBox.text = desc;
    //}

    private void EditStatByIndex(string text, int index)
    {
        slotStats[index].text = text;
        slotStats[index].gameObject.SetActive(true);
        if (text == "0") slotStats[index].text = "";
    }

    private void SetIconInfoByIndex(InfoType type, int index)
    {
        if (iconDictionary.TryGetValue(type, out Sprite sprite))
        {
            icons[index].sprite = sprite;
            icons[index].gameObject.SetActive(true);
        }
    }

    private void WriteStat(InfoType type, string text, int index)
    {
        EditStatByIndex(text, index);
        SetIconInfoByIndex(type, index);
    }

    private void HideUnusedSlots()
    {
        foreach (var icon in icons)
        {
            if (icon.sprite == null) icon.gameObject.SetActive(false);
        }
    }

    //private void HideUnusedSlots(int usedCount)
    //{
    //    for (int i = usedCount; i < slotStats.Count; i++)
    //    {
    //        slotStats[i].gameObject.SetActive(false);
    //    }
    //}

    private void ResetStatSlots()
    {
        for (int i = 0; i < slotStats.Count; i++)
        {
            slotStats[i].gameObject.SetActive(false);
        }
    }

    private void SetIconItem(Sprite sprite)
    {
        iconItem.sprite = sprite;
    }

    private string FormatTierValue(int t1, int t2)
    {
        return t1 != t2 ? $"{t1}<color=green>→{t2}</color>" : $"{t1}";
    }

    private IEnumerable<InfoType> GetGearStats(Gear gear)
    {
        if (gear.hasHPStat) yield return InfoType.HP;
        if (gear.hasATKStat) yield return InfoType.ATK;
        if (gear.hasSPStat) yield return InfoType.SP;
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

    public void DisplayGearByTier(Gear gear, int tier)
    {
        SetupHeader(gear.itemName, "Gear", gear.sprite);
        EditTierTextBox($"Tier {tier}");
        tierTextBox.gameObject.SetActive(true);

        ResetStatSlots();
        int index = 0;

        foreach(var stat in GetGearStats(gear))
        {
            string text = gear.GetStats(stat, tier).ToString();
            WriteStat(stat, text, index++);
        }

        WriteStat(InfoType.EXPIRATION, gear.Expiration.ToString(), index);
        HideUnusedSlots();
    } 

    public void DisplayEquippedGear(Gear gear)
    {
        DisplayGearByTier(gear, gear.CurrentTier);
    }

    private void DisplayRangeItem(Range range)
    {
        SetupHeader(range.itemName, "Weapon", range.sprite);
        InfoType[] stats = 
        { 
            InfoType.DAMAGE,
            InfoType.PROJECTILES,
            InfoType.FIRE_RATE
        };

        int index = 0;
        foreach( var type in stats)
        {
            int T1 = (int)range.GetProperty(type, 1);
            int T2 = (int)range.GetProperty(type, 2);

            WriteStat(type, FormatTierValue(T1, T2), index++);
        }

        HideUnusedSlots();
    }

    private void DisplayMeleeItem(Melee melee)
    {
        SetupHeader(melee.itemName, "Weapon", melee.sprite);

        InfoType[] stats =
        {
            InfoType.DAMAGE,
            InfoType.SLASH_INTERVAL,
        };

        int index = 0;
        foreach (var type in stats)
        {
            int T1 = (int)melee.GetProperty(type, 1);
            int T2 = (int)melee.GetProperty(type, 2);

            WriteStat(type, FormatTierValue(T1, T2), index++);
        }

        HideUnusedSlots();
    }

    private void DisplayGearItem(Gear gear)
    {
        SetupHeader(gear.itemName, "Gear", gear.sprite);

        int index = 0;

        foreach (var stat in GetGearStats(gear))
        {
            int t1 = gear.GetStats(stat, 1);
            int t2 = gear.GetStats(stat, 2);

            WriteStat(stat, FormatTierValue(t1, t2), index++);
        }

        WriteStat(InfoType.EXPIRATION, gear.Expiration.ToString(), index);

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
