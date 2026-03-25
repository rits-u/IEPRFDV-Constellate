using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : ScreenUI
{
    [SerializeField] private PlayerPanel P1;
    [SerializeField] private PlayerPanel P2;

    [System.Serializable]
    public class PlayerPanel
    {
        public ProfileUI profileUI;
        public WeaponUI weaponUI;
        public GearUI gearUI;
    }


    [System.Serializable]
    public class ProfileUI
    {
        public GameObject iconSlot;
        public TextMeshProUGUI name;
        public TextMeshProUGUI[] stats;
    }

    [System.Serializable]
    public class WeaponUI
    {
        public GameObject iconSlot;
        public TextMeshProUGUI name;
        public TextMeshProUGUI tier;
        public GameObject meleeGroup;
        public GameObject rangeGroup;
        public TextMeshProUGUI[] melee;
        public TextMeshProUGUI[] range;
    }

    [System.Serializable]
    public class GearUI
    {
        public GearSlot[] gearSlots;
    }

    [System.Serializable]
    public class GearSlot
    {
        public GameObject iconSlot;
        public TextMeshProUGUI expire;
        public TextMeshProUGUI tier;

        public void SetActive(bool value)
        {
            iconSlot.SetActive(value);
            expire.gameObject.SetActive(value);
            tier.gameObject.SetActive(value);
        }
    }

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);
        for (int i = 1; i <= 2; i++) //update on both players
        {
            UpdateProfileUI(i);
            UpdateWeaponUI(i);
            UpdateGearUI(i);
        }
    }

    public override void HideScreenUI()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        screenName = "Inventory";
    }

    private void UpdateProfileUI(int ID)
    {
        PlayerPanel panel = ID == 1 ? P1 : P2;
        Player player = PlayerManager.Instance.GetPlayerByID(ID);
        Stats playerStats = player.GetComponent<Stats>();

        Image icon = panel.profileUI.iconSlot.transform.GetChild(0).GetComponent<Image>();
        icon.sprite = player.avatar;
        //icon.sprite = charIcon;

        panel.profileUI.name.text = player.Name;

        panel.profileUI.stats[0].text = $"HP : {playerStats.HP}/{playerStats.MaxHP}";
        panel.profileUI.stats[1].text = $"SP : {playerStats.SP}";
        panel.profileUI.stats[2].text = $"ATK: {playerStats.ATK}";
    }

    private void UpdateWeaponUI(int ID)
    {
        PlayerPanel panel = ID == 1 ? P1 : P2;
        Weapon weapon = PlayerManager.Instance.GetPlayerWeapon(ID);

        Image icon = panel.weaponUI.iconSlot.transform.GetChild(0).GetComponent<Image>();
        icon.sprite = weapon.sprite;

        panel.weaponUI.name.text = weapon.itemName;     //item

        panel.weaponUI.tier.text = $"Tier {weapon.CurrentTier}";    //tier

        if (weapon.IsMelee)
        {
            Melee m = (Melee)weapon;
            panel.weaponUI.meleeGroup.gameObject.SetActive(true);
            panel.weaponUI.rangeGroup.gameObject.SetActive(false);

            panel.weaponUI.melee[0].text = $"DMG: {m.GetPropertyByType(InfoType.DAMAGE)}";
            panel.weaponUI.melee[1].text = $"SPD: {m.GetPropertyByType(InfoType.SLASH_INTERVAL)}";
        }
        else
        {
            Range r = (Range)weapon;
            panel.weaponUI.meleeGroup.gameObject.SetActive(false);
            panel.weaponUI.rangeGroup.gameObject.SetActive(true);

            panel.weaponUI.range[0].text = $"DMG: {r.GetPropertyByType(InfoType.DAMAGE)}";
            panel.weaponUI.range[1].text = $"BUL: {r.GetPropertyByType(InfoType.PROJECTILES)}";
            panel.weaponUI.range[2].text = $"FRT: {r.GetPropertyByType(InfoType.FIRE_RATE)}";
        }
    }

    private void UpdateGearUI(int ID)
    {
        PlayerPanel panel = ID == 1 ? P1 : P2;
        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(ID);
        int count = inventory.GetEquippedGearCount();

        for (int i = 0; i < count; i++)
        {
            Gear gear = inventory.GetEquippedGearByIndex(i).gear;
            panel.gearUI.gearSlots[i].SetActive(true);

            Image icon = panel.gearUI.gearSlots[i].iconSlot.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = gear.sprite;
            icon.GetComponent<ItemHover>().Initialize(ID, i);

            panel.gearUI.gearSlots[i].expire.text = $"{gear.Expiration}";   //expiration

            panel.gearUI.gearSlots[i].tier.text = $"Tier {gear.CurrentTier}";    //tier
        }

        for(int i = count; i < inventory.GetMaxSlots(); i++)
        {
            panel.gearUI.gearSlots[i].SetActive(false);
        }

    }

}
