using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiscardUI : ScreenUI
{
    [Header("Main Panels")]
    [SerializeField] private Transform displayPanel;
    [SerializeField] private Transform miniInventory;

    [Header("Buttons")]
    [SerializeField] private Button skipButton;
    [SerializeField] private Button closeButton;

   // [SerializeField] private GameObject displayPrefab;

    private List<ItemDisplay> itemDisplays = new();
    private List<Gear> copyLoots = new();
    private System.Action discardCallback;

    private int playerID;

    private int attemptIndex; //index for gear to discard
    private int tierLoot;

    private void Start()
    {
        screenName = "Discard";
    }

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);    
    }

    public override void HideScreenUI()
    {
        gameObject.SetActive(false);
    }

    private void InitializeWindow()
    {
        skipButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);
    }


    public void InitializeDisplayPanel(List<Gear> gears) 
    {
        InitializeWindow();
        copyLoots.Clear();
        copyLoots = gears;
        ResetDisplays();

        foreach (var gear in gears)
        {
            ItemDisplay display = UIManager.Instance.CreateItemDisplay(displayPanel, 0.45f);
            display.DisplayEquippedGear(gear);
            itemDisplays.Add(display);
        }
    }


    public void OpenDiscardWindow(int ID, int toDiscard, int tier, System.Action onFinished)
    {
        playerID = ID;
        tierLoot = tier;
        discardCallback = onFinished;

        Debug.Log("Opened Discard Window");
        EnableAllDiscardButtons();
        UpdateMiniInventory(ID);
        
        itemDisplays[0].SelectDisplay();
    }

    public void OnSkipButtonPressed()
    {
        SelectNextLoot();
    }

    public void CloseDiscardWindow()
    {
        HideScreenUI();
        UIManager.Instance.RemoveDim();
        discardCallback?.Invoke();
    }

    public void SelectGearToDiscard(int buttonID)
    {
        attemptIndex = buttonID - 1;
        SwitchToConfirmationPanel();
        Transform panel = miniInventory.GetChild(2);
        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        Gear selected = inventory.GetEquippedGearByIndex(attemptIndex).gear;

        TextMeshProUGUI statement = panel.GetChild(0).GetComponent<TextMeshProUGUI>();
        string equippedGear = $"<b><color=red>{selected.itemName}</color></b>";
        string lootGear = $"<b><color=blue>{copyLoots[0].itemName}</color></b>";
        statement.text = $"Discard [ {equippedGear} ] for [ {lootGear} ] ?";
    }

    public void CancelGearDiscard()
    {
        SwitchBackToGearPanel();
    }

    //activated by yes button at confirmation panel
    public void DiscardAndReplaceGear()
    {
        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        inventory.UnequipGearByIndex(attemptIndex);

        inventory.EquipGear(copyLoots[0], tierLoot);    //replacement

        SelectNextLoot();
        SwitchBackToGearPanel();
    }

    private void RemoveSelectedDisplay()
    {
        ItemDisplay display = itemDisplays[0];
        itemDisplays.RemoveAt(0);
        Destroy(display.gameObject);

    }

    private void SelectNextLoot()
    {
        if (copyLoots.Count > 0)
        {
            RemoveSelectedDisplay();
            copyLoots.RemoveAt(0);
        }
        
        if(copyLoots.Count <= 0)
        { 
            skipButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);

            //disable all discard buttons
            DisableAllDiscardButtons();
        }
    }

    private void DisableAllDiscardButtons()
    {
        Transform gearPanel = miniInventory.GetChild(1);
        for (int i = 0; i < gearPanel.childCount; i++)
        {
            Button discardBtn = gearPanel.GetChild(i).GetComponentInChildren<Button>();
            discardBtn.gameObject.SetActive(false);
        }
    }

    private void EnableAllDiscardButtons()
    {
        Transform gearPanel = miniInventory.GetChild(1);
        for (int i = 0; i < gearPanel.childCount; i++)
        {
            Button discardBtn = gearPanel.GetChild(i).GetComponentInChildren<Button>(true);
            discardBtn.gameObject.SetActive(true);
        }
    }


    private void UpdateMiniInventory(int ID)
    {
        UpdateProfilePanel(ID);
        UpdateGearPanel(ID);
    }
    
    private void UpdateProfilePanel(int ID)
    {
        Stats player = PlayerManager.Instance.GetPlayerByID(ID).GetComponent<Stats>();

        Transform profile = miniInventory.GetChild(0);
        Image charIcon = profile.GetChild(0).GetComponent<Image>();
        // charIcon.sprite = change sprite

        Transform statsGroup = profile.GetChild(1);
        List<TextMeshProUGUI> stats = new();
        for (int i = 0; i < statsGroup.childCount; i++)
        {
            stats.Add(statsGroup.GetChild(i).GetComponent<TextMeshProUGUI>());
        }

        stats[0].text = $"HP: {player.HP}/{player.MaxHP}";
        stats[1].text = $"SP: {player.SP}";
        stats[2].text = $"ATK: {player.ATK}";
    }

    private void UpdateGearPanel(int ID)
    {
        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(ID);
        int equipped = inventory.GetEquippedGearCount();


        Transform gearPanel = miniInventory.GetChild(1);
        List<Transform> gearSlots = new();
        for (int i = 0; i < gearPanel.childCount; i++)
        {
            gearSlots.Add(gearPanel.GetChild(i).GetComponent<Transform>());
        }

       
        for(int i = 0; i < equipped; i++)
        {
            Gear gear = inventory.GetEquippedGearByIndex(i).gear;
            Transform slot = gearPanel.GetChild(i);
            TextMeshProUGUI expire = slot.GetChild(0).GetComponent<TextMeshProUGUI>();
            expire.text = $"{gear.Expiration}";

            TextMeshProUGUI tier = slot.GetChild(1).GetComponent<TextMeshProUGUI>();
            tier.text = $"Tier {gear.CurrentTier}";

            Image icon = slot.GetChild(2).GetComponent<Image>();
            icon.sprite = gear.sprite;
            icon.GetComponent<ItemHover>().Initialize(ID, i);

            TextMeshProUGUI name = slot.GetChild(3).GetComponent<TextMeshProUGUI>();
            name.text = gear.itemName;
        }

        for(int i = equipped; i < gearSlots.Count; i++)
        {
            Transform slot = gearPanel.GetChild(i);
            slot.gameObject.SetActive(false);
        }
    }

    private void SwitchToConfirmationPanel()
    {
        miniInventory.GetChild(1).gameObject.SetActive(false);
        miniInventory.GetChild(2).gameObject.SetActive(true);
    }

    private void SwitchBackToGearPanel()
    {
        UpdateMiniInventory(playerID);
        miniInventory.GetChild(1).gameObject.SetActive(true);
        miniInventory.GetChild(2).gameObject.SetActive(false);
    }

    private void ResetDisplays()
    {
        foreach(var display in itemDisplays)
        {
            Destroy(display.gameObject);
        }

        itemDisplays.Clear();
    }
}
