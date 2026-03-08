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
        playerStats.MaxHP += gear.HP * tier;
        playerStats.ATK += gear.ATK * tier;
        playerStats.SP += gear.SP * tier;
        
        gearList.Add(new GearInfo(gear, tier));
        Debug.Log($"{gameObject.name} equipped {gear.itemName}");
    }

    public void UnEquipGear(Gear gear)
    {
        playerStats.MaxHP -= gear.HP;
        playerStats.ATK -= gear.ATK;
        playerStats.SP -= gear.SP;

        int index = 0;
        foreach (var g in gearList)
        {
            if (g.gear == gear) break;
            index++;
        }

        gearList.RemoveAt(index);


    }

    public void EquipGun(Gun newGun, int tier)
    {
        Gun temp = ScriptableObject.CreateInstance<Gun>();
        temp.Damage = newGun.Damage;
        temp.NumBullets = newGun.NumBullets;

        if (tier > 1)
            temp.Upgrade(tier);

        gun = temp;       
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
