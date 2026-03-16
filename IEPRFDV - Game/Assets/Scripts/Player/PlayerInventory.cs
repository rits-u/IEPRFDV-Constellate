using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{

    [Header("Weapon")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private WeaponObject weaponObj;
    //[SerializeField] private Gun gun;
    //[SerializeField] private Melee melee;
  //  [SerializeField] pri

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

    //public Gun GetPlayerGunWeapon()
    //{
    //    return gun;
    //}

    //public Melee GetPlayerMeleeWeapon()
    //{
    //    return melee;
    //}

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
        playerStats.ATK += copy.GetStatsByType(InfoType.ATK);
        playerStats.SP += copy.GetStatsByType(InfoType.SP);
        playerStats.UpdateHealthBar();
        
        gearList.Add(new GearInfo(copy, tier));
        Debug.Log($"{gameObject.name} equipped {gear.itemName}");
    }


    ////fix, discard
    //public void UnequipGear(Gear gear)
    //{
    //    int index = 0;
    //    foreach (var g in gearList)
    //    {
    //        if (g.gear == gear) break;
    //        index++;
    //    }

    //    gearList.RemoveAt(index);


    //}

    public void UnequipGearByIndex(int index)
    {
        Gear copy = gearList[index].gear;
        playerStats.MaxHP -= copy.GetStatsByType(InfoType.HP);
        playerStats.SP -= copy.GetStatsByType(InfoType.SP);
        playerStats.ATK -= copy.GetStatsByType(InfoType.ATK);
        playerStats.UpdateHealthBar();

        gearList.RemoveAt(index);

        Debug.Log($"{gameObject.name} has unequipped {copy.itemName}");
    }

    //public void EquipGun(Gun gunToEquip, int tier)
    //{
    //    Gun copy = Instantiate(gunToEquip);
    //    copy.CurrentTier = tier;
    //    gun = copy;
    //}

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
}
