using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI nameTextBox;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private Image iconItem;
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

    public void EditStatByIndex(string text, int index)
    {
        slotStats[index].text = text;
        slotStats[index].gameObject.SetActive(true);
        if (text == "0") slotStats[index].text = "";
    }

    public void EditNameTextBox(string name) { 
        nameTextBox.text = name;
    }

    public void EditTypeTextBox(string type)
    {
        itemType.text = type;
    }

    //public void EditDescTextBox(string desc)
    //{
    //    descTextBox.text = desc;
    //}

    public void SetIconInfoByIndex(InfoType type, int index)
    {
        if (iconDictionary.TryGetValue(type, out Sprite sprite))
        {
            icons[index].sprite = sprite;
            icons[index].gameObject.SetActive(true);
        }
    }

    public void HideUnusedSlots()
    {
        foreach(var icon in icons)
        {
            if(icon.sprite == null) icon.gameObject.SetActive(false);
        }
    }

    public void SetIconItem(Sprite sprite)
    {
        iconItem.sprite = sprite;
    }


}
