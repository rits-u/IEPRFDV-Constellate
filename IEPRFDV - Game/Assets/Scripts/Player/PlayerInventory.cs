using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{

    [Header("Weapon")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponObject weaponObj;

    [Header("Gear")]
    [SerializeField] private int maxSlots;
    [SerializeField] private List<GearInfo> gearList = new();

    public event Action OnInventoryChanged;
    //gear bar listens

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

    private void OnEnable()
    {
        InitializeWeapon();
    }

    private void Start()
    {
        playerStats = GetComponent<Stats>();

        
    }

    private void InitializeWeapon()
    {
        weaponObj.weapon = weapon;
        weaponObj.SetWeaponSprite(weapon.IsMelee, weapon.sprite);
        if (weapon.IsMelee)
        {
            weaponObj.SwitchToMelee();
        }
        else
        {
            weaponObj.SwitchToRange();
        }
        
    }

    public Weapon GetPlayerWeapon()
    {
        return weapon;
    }

    public List<GearInfo> GetAllEquippedGears()
    {
        return gearList;
    }

    public GearInfo GetEquippedGearByIndex(int index)
    {
        return gearList[index];
    }

    public void EquipGear(Gear gear, int tier)
    {
        Gear copy = Instantiate(gear);
        copy.CurrentTier = tier;
        playerStats.MaxHP += copy.GetStatsByType(InfoType.HP);
        playerStats.HP += copy.GetStatsByType(InfoType.HP);     //adjust hp
        playerStats.ATK += copy.GetStatsByType(InfoType.ATK);
        playerStats.SP += copy.GetStatsByType(InfoType.SP);
//        playerStats.ValidateStats();
        playerStats.UpdateBar();
        
        gearList.Add(new GearInfo(copy, tier));
        OnInventoryChanged?.Invoke();
        Debug.Log($"{gameObject.name} equipped {gear.itemName}");
    }


    public void UnequipGearByIndex(int index)
    {
        Gear copy = gearList[index].gear;
        playerStats.MaxHP -= copy.GetStatsByType(InfoType.HP);
        playerStats.SP -= copy.GetStatsByType(InfoType.SP);
        playerStats.ATK -= copy.GetStatsByType(InfoType.ATK);
        playerStats.ValidateStats();
        playerStats.UpdateBar();

        gearList.RemoveAt(index);
        OnInventoryChanged?.Invoke();
        Debug.Log($"{gameObject.name} has unequipped {copy.itemName}");
    }


    public void ReplaceGear(Gear newGear, int tier, int replaceIndex)
    {
        Gear oldGear = gearList[replaceIndex].gear;

        Gear newCopy = Instantiate(newGear);
        newCopy.CurrentTier = tier;

        //difference
        int hpDiff = newCopy.GetStatsByType(InfoType.HP) - oldGear.GetStatsByType(InfoType.HP);
        int atkDiff = newCopy.GetStatsByType(InfoType.ATK) - oldGear.GetStatsByType(InfoType.ATK);
        int spDiff = newCopy.GetStatsByType(InfoType.SP) - oldGear.GetStatsByType(InfoType.SP);

        playerStats.MaxHP += hpDiff;
        playerStats.ATK += atkDiff;
        playerStats.SP += spDiff;

        //adjust
        playerStats.HP += hpDiff;
        playerStats.HP = Mathf.Clamp(playerStats.HP, 1, playerStats.MaxHP); //clamp

        gearList[replaceIndex] = new GearInfo(newCopy, tier);

        playerStats.ValidateStats();
        playerStats.UpdateBar();
        OnInventoryChanged?.Invoke();
        Debug.Log($"{gameObject.name} replaced {oldGear.itemName} with {newGear.itemName}");

    }

    public void UpdateGearExpirations()
    {
        int index = 0;
        List<int> expired = new();
        foreach (GearInfo info in gearList)
        {
            info.gear.Expiration--;
            if(info.gear.Expiration <= 0 )
            {
                expired.Add(index);
            }
            index++;
        }
    //    expired.Sort((a, b) => b.CompareTo(a));
        for(int i = expired.Count -1; i >= 0; i--)
        {
            UnequipGearByIndex(expired[i]);
        }
    }


    public void EquipWeapon(Weapon weaponToEquip, int tier)
    {
        Weapon copy = Instantiate(weaponToEquip);
        copy.CurrentTier = tier;
        weapon = copy;

        InitializeWeapon();

        weaponObj.SetWeaponSprite(copy.IsMelee, copy.sprite);
    }

    public void UnequipCurrentWeapon()
    {
        if (weapon.IsMelee) Destroy(weaponObj.GetComponent<BasicMeleeBehavior>());
        else Destroy(weaponObj.GetComponent<BasicRangeBehavior>());
    }

    public int GetEquippedGearCount()
    {
        return gearList.Count;
    }

    public int GetMaxSlots()
    {
        return maxSlots;
    }

    public void DisableWeapon()
    {
        if(weapon is Range)
        {
            weaponObj.GetComponent<BasicRangeBehavior>().enabled = false;
            weaponObj.GetComponent<BasicRangeBehavior>().StopAllCoroutines();
        }
        else
        {
            weaponObj.GetComponent<BasicMeleeBehavior>().enabled = false;
            weaponObj.GetComponent<BasicMeleeBehavior>().StopAllCoroutines();
        }
    }

    public void EnableWeapon()
    {
        if (weapon is Range)
        {
            weaponObj.GetComponent<BasicRangeBehavior>().enabled = true;
        }
        else
        {
            weaponObj.GetComponent<BasicMeleeBehavior>().enabled = true;
        }
    }
}
