using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GearBar : MonoBehaviour
{
    [Header("Player Object")]
    [SerializeField] private Player player;

    private List<Transform> gearSlots = new();

    private PlayerInventory inventory;

    private void OnEnable()
    {
        inventory = player.GetComponent<PlayerInventory>();
        inventory.OnInventoryChanged += UpdateGearSlots;
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= UpdateGearSlots;
    }

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            gearSlots.Add(transform.GetChild(i));
        }
       
        
    }

    //gets called whenever there is a change in player's inventory
    public void UpdateGearSlots()
    {
        int equipped = inventory.GetEquippedGearCount();
        for (int i = 0; i < equipped; i++)
        {
            Image icon = gearSlots[i].GetChild(0).GetComponent<Image>();
            icon.sprite = inventory.GetEquippedGearByIndex(i).gear.sprite;
            FillAlpha(icon);
        }

        for(int i = equipped; i < gearSlots.Count; i++)
        {
            Image icon = gearSlots[i].GetChild(0).GetComponent<Image>();
            EmptyAlpha(icon);
        }
    }

    private void FillAlpha(Image icon)
    {
        Color temp = icon.color;
        temp.a = 1f;
        icon.color = temp;
    }

    private void EmptyAlpha(Image icon)
    {
        Color temp = icon.color;
        temp.a = 0f;
        icon.color = temp;
    }
}
