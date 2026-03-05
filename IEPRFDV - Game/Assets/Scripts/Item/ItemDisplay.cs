using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.UI;
using Unity.UIElements;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public enum InfoType
{
    HP,
    ATK,
    SP,
    DAMAGE,
    BULLETS
}

public class ItemDisplay : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> numTextBoxes;
    [SerializeField] private TextMeshProUGUI nameTextBox;
    [SerializeField] private TextMeshProUGUI descTextBox;

    private static Dictionary<InfoType, Sprite> iconDictionary;
    [SerializeField] private List<Image> icons;

    private void Awake()
    {
        iconDictionary = new Dictionary<InfoType, Sprite>();

        foreach (var pair in iconMappings)
        {
            if (!iconDictionary.ContainsKey(pair.type))
                iconDictionary.Add(pair.type, pair.sprite);
        }
    }

    public void EditTextBoxByIndex(string text, int index)
    {
        numTextBoxes[index].text = text;
        numTextBoxes[index].gameObject.SetActive(true);
        if (text == "0") numTextBoxes[index].text = "";
    }

    public void EditNameTextBox(string name) { 
        nameTextBox.text = name;
    }

    public void EditDescTextBox(string desc)
    {
        descTextBox.text = desc;
    }

    public void SetIconByIndex(InfoType type, int index)
    {
        if (iconDictionary.TryGetValue(type, out Sprite sprite))
        {
            icons[index].sprite = sprite;
            icons[index].gameObject.SetActive(true);
        }
    }

    [SerializeField] private List<StatIconPair> iconMappings;

    [System.Serializable]
    public struct StatIconPair
    {
        public InfoType type;
        public Sprite sprite;

        //public StatIconPair(InfoType type, int value)
        //{
        //    this.type = type;
        //    this.value = value;
        //}

    }

}
