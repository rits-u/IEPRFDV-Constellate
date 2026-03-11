using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{

    [Header("Weapon")]
    [SerializeField] private Gun gun;

    [Header("Gear")]
    [SerializeField] private int maxSlots;
    [SerializeField] private List<GearInfo> gearList = new();

    //[Header("UI Elements")]
    private Stats playerStats;

    [System.Serializable]
    public struct GearInfo{
        public Gear gear;
        public int tier;

        public GearInfo(Gear gear, int tier)
        {
            this.gear = gear;
            this.tier = tier;
        }
    }

    private void Start()
    {
        playerStats = GetComponent<Stats>();
    }

    public Gun GetPlayerGun()
    {
        return gun;
    }

    public void EquipGear(Gear gear, int tier)
    {
        Gear copy = Instantiate(gear);
        copy.CurrentTier = tier;
        playerStats.MaxHP += copy.GetStatsByType(InfoType.HP);
        playerStats.ATK += copy.GetStatsByType(InfoType.ATK);
        playerStats.SP += copy.GetStatsByType(InfoType.SP);
        playerStats.UpdateHealthBar();
        
        gearList.Add(new GearInfo(copy, tier));
        Debug.Log($"{gameObject.name} equipped {gear.itemName}");
    }


    //fix, discard
    public void UnEquipGear(Gear gear)
    {
        //playerStats.MaxHP -= gear.HP;
        //playerStats.ATK -= gear.ATK;
        //playerStats.SP -= gear.SP;

        int index = 0;
        foreach (var g in gearList)
        {
            if (g.gear == gear) break;
            index++;
        }

        gearList.RemoveAt(index);


    }

    public void EquipGun(Gun gunToEquip, int tier)
    {
        Gun copy = Instantiate(gunToEquip);
        copy.CurrentTier = tier;
        gun = copy;
    }

    public int GetEquippedGearCount()
    {
        return gearList.Count;
    }

    public int GetMaxSlots()
    {
        return maxSlots;
    }
}
