using Mono.Cecil;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class InventoryUI : ScreenUI
{
    [Header("UI Elements")]
    [SerializeField] private List<PlayerPanel> players = new();
    [SerializeField] private Button closeButton;

    private List<Transform> gearSlots = new();

    [System.Serializable]
    private class PlayerPanel
    {
        public Transform root;
        private PanelUI profile;
        private PanelUI weapon;
        private PanelUI gear;

        public PanelUI Profile 
        {
            get => profile;
            set => profile = value;
        }

        public PanelUI Weapon 
        {
            get => weapon;
            set => weapon = value;
        }
        public PanelUI Gear
        {
            get => gear;
            set => gear = value;
        }
    }

    private void Awake()
    {
        foreach (var player in players)
        {
            player.Profile = player.root.GetChild(0).GetComponent<PanelUI>();
            player.Weapon = player.root.GetChild(1).GetComponent<PanelUI>();
            player.Gear = player.root.GetChild(2).GetComponent<PanelUI>();   
        }
    }

    private void Start()
    {
        screenName = "Inventory";
    }

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);
        
        UpdateProfileUI(1);
        UpdateWeaponUI(1);
        UpdateGearUI(1);
    }

    public override void HideScreenUI()
    {
        gameObject.SetActive(false);
    }

    private void UpdateProfileUI(int ID)
    {
        int i = ID - 1;
        PanelUI profileUI = players[i].root.GetChild(0).GetComponent<PanelUI>();
        Stats player = PlayerManager.Instance.GetPlayerByID(ID).GetComponent<Stats>();
        if (profileUI == null || player == null) return;

        //Icon
        Image profileIcon = profileUI.transform.GetChild(0).GetComponent<Image>();
        //profileIcon.sprite =  //insert player icon

        //stats 
        var statsGroup = profileUI.transform.GetChild(1); 
        List<TextMeshProUGUI> statsList = new();
        for(int j = 0; j < statsGroup.transform.childCount; j++)
        {
            statsList.Add(statsGroup.transform.GetChild(j).GetComponent<TextMeshProUGUI>());
        }

        statsList[0].text = $"HP : {player.HP}/{player.MaxHP}";
        statsList[1].text = $"SP : {player.SP}";
        statsList[2].text = $"ATK: {player.ATK}";

    }

    private void UpdateWeaponUI(int ID)
    {
        int i = ID - 1;
        PanelUI weaponUI = players[i].root.GetChild(1).GetComponent<PanelUI>();
        Weapon weapon = PlayerManager.Instance.GetPlayerWeapon(ID);
        if (weaponUI == null || weapon == null) return;

        //icon
        Image weaponIcon = weaponUI.transform.GetChild(0).GetComponent<Image>();
        weaponIcon.sprite = weapon.sprite;

        //name
        TextMeshProUGUI nameTextBox = weaponUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameTextBox.text = weapon.name;

        //properties
        Transform statsGroup = weapon.IsMelee ? weaponUI.transform.GetChild(2) : weaponUI.transform.GetChild(3);
        
      //  statsGroup = weaponUI.transform.GetChild(2);
        List<TextMeshProUGUI> statsList = new();
        for (int j = 0; j < statsGroup.transform.childCount; j++)
        {
            statsList.Add(statsGroup.transform.GetChild(j).GetComponent<TextMeshProUGUI>());
        }

        if (weapon.IsMelee)
        {
            Melee melee = (Melee)weapon;
            statsList[0].text = $"DMG: {melee.GetPropertyByType(InfoType.DAMAGE)}";
            statsList[1].text = $"SPD: {melee.GetPropertyByType(InfoType.SLASH_INTERVAL)}";
            weaponUI.transform.GetChild(2).gameObject.SetActive(true);
            weaponUI.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            Range range = (Range)weapon;
            statsList[0].text = $"DMG: {range.GetPropertyByType(InfoType.DAMAGE)}";
            statsList[1].text = $"BUL: {range.GetPropertyByType(InfoType.PROJECTILES)}";
            statsList[1].text = $"FRT: {range.GetPropertyByType(InfoType.FIRE_RATE)}";
            weaponUI.transform.GetChild(3).gameObject.SetActive(true);
            weaponUI.transform.GetChild(2).gameObject.SetActive(false);
        }

    }

    private void UpdateGearUI(int ID)
    {
        int i = ID - 1;
        PanelUI gearUI = players[i].root.GetChild(2).GetComponent<PanelUI>();
        if (gearUI == null) return;

        Transform gearGroup = gearUI.transform.GetChild(0);
        gearSlots.Clear();
        for (int j = 0; j < gearGroup.childCount; j++)
        {
            gearSlots.Add(gearGroup.GetChild(j).GetComponent<Transform>());
        }
        // GetOccupiedGearSlots(i);

        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(ID);
        int equipped = inventory.GetEquippedGearCount();

        for (int j = 0; j < equipped; j++)
        {
            var info = inventory.GetEquippedGearByIndex(j);
            int tier = info.tier;

            Gear gear = info.gear;
            Image gearIcon = gearSlots[j].GetChild(0).GetComponent<Image>();    //icon
            gearIcon.sprite = gear.sprite;
            gearIcon.gameObject.SetActive(true);

            TextMeshProUGUI expire = gearSlots[j].GetChild(1).GetComponent<TextMeshProUGUI>();  //expiration
            expire.text = gear.Expiration.ToString();
            expire.gameObject.SetActive(true);

        }

        //hide unused slots
        for(int j = equipped; j < gearSlots.Count; j++)
        {
            gearSlots[j].GetChild(0).gameObject.SetActive(false);
            gearSlots[j].GetChild(1).gameObject.SetActive(false);
        }

        HideDiscardButtons();
    }

    private void HideDiscardButtons()
    {
        for(int i = 0; i < gearSlots.Count; i++)
        {
            gearSlots[i].GetChild(2).gameObject.SetActive(false);
        }
    }

    public void HideCloseButton()
    {
        closeButton.gameObject.SetActive(false);
    }

    public void ShowCloseButton()
    {
        closeButton.gameObject.SetActive(false);
    }

    private void GetOccupiedGearSlots(int index)
    {
        PanelUI gearUI = players[index].root.GetChild(2).GetComponent<PanelUI>();
        Transform gearGroup = gearUI.transform.GetChild(0);
        //Debug.Log("chidsald: " + gearGroup.childCount);
        gearSlots.Clear();
        for (int j = 0; j < gearGroup.childCount; j++)
        {
            gearSlots.Add(gearGroup.GetChild(j).GetComponent<Transform>());
        }
    }

    public void DiscardWindow()
    {
        HideCloseButton();
        ShowScreenUI();

        for (int i = 0; i < players.Count; i++)
        {
            PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(i + 1);
            int equipped = inventory.GetEquippedGearCount();

            for (int j = 0; j < equipped; j++)
            {
                gearSlots[j].GetChild(3).gameObject.SetActive(true);
            }
        }


    }
}
