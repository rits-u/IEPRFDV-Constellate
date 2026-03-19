using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OLDDiscardUI : ScreenUI
{
    [Header("Main Panels")]
    [SerializeField] private Transform displayPanel;
    [SerializeField] private Transform miniInventory;

    [Header("Buttons")]
    [SerializeField] private Button skipButton;
    [SerializeField] private Button closeButton;

    private List<ItemDisplay> itemDisplays = new();
    private List<Gear> copyLoots;

    private System.Action<List<Gear>> discardCallback;

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

    public void Initialize(List<Gear> gears)
    {
        InitializeWindow();
        // Debug.Log("gear COUNT: " + gears.Count);
        copyLoots = new List<Gear>(gears);
        ResetDisplays();
    }

    public void InitializeDisplayPanel(int tier)
    {
        foreach (var gear in copyLoots)
        {
            ItemDisplay display = UIManager.Instance.CreateItemDisplay(displayPanel, 0.45f);
            display.DisplayGearByTier(gear, tier);
            itemDisplays.Add(display);
        }
    }


    public void OpenDiscardWindow(int ID, int tier, System.Action<List<Gear>> onFinished)
    {
        playerID = ID;
        tierLoot = tier;
        discardCallback = onFinished;

        Debug.Log("Opened Discard Window");
        InitializeDisplayPanel(tier);
        PlayerManager.Instance.DisableAllPlayerMovement();
        EnableAllDiscardButtons();
        UpdateMiniInventory(ID);

        itemDisplays[0].SelectDisplay();
    }


    public void CloseDiscardWindow()
    {
        HideScreenUI();
        UIManager.Instance.RemoveDim();
        discardCallback?.Invoke(copyLoots);
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

    public void SkipAttempt()
    {
        SwitchToConfirmSkipPanel();
        skipButton.gameObject.SetActive(false);
    }

    public void ConfirmedSkipGear()
    {
        SwitchBackToGearPanel();
        skipButton.gameObject.SetActive(true);
        SelectNextLoot();
    }


    public void CancelGearDiscard()
    {
        SwitchBackToGearPanel();
        skipButton.gameObject.SetActive(true);
    }

    //activated by yes button at confirmation panel
    public void DiscardAndReplaceGear()
    {
        PlayerInventory inventory = PlayerManager.Instance.AccessPlayerInventory(playerID);
        inventory.ReplaceGear(copyLoots[0], tierLoot, attemptIndex);

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

        if (copyLoots.Count <= 0)
        {
            skipButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);

            //disable all discard buttons
            DisableAllDiscardButtons();
            SwitchBackToGearPanel();
        }
        else
        {
            itemDisplays[0].SelectDisplay();
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
        Player player = PlayerManager.Instance.GetPlayerByID(ID);
        Stats playerStats = player.GetComponent<Stats>();

        Transform profile = miniInventory.GetChild(0);
        TextMeshProUGUI idNum = profile.GetChild(0).GetComponent<TextMeshProUGUI>();
        idNum.text = player.ID == 1 ? $"<color=red>P{player.ID}</color>" : $"<color=blue>P{player.ID}</color>";

        Image charIcon = profile.GetChild(1).GetComponent<Image>();
        // charIcon.sprite = change sprite

        TextMeshProUGUI name = profile.GetChild(2).GetComponent<TextMeshProUGUI>();
        name.text = player.Name;

        Transform statsGroup = profile.GetChild(3);
        List<TextMeshProUGUI> stats = new();
        for (int i = 0; i < statsGroup.childCount; i++)
        {
            stats.Add(statsGroup.GetChild(i).GetComponent<TextMeshProUGUI>());
        }

        stats[0].text = $"HP: {playerStats.HP}/{playerStats.MaxHP}";
        stats[1].text = $"SP: {playerStats.SP}";
        stats[2].text = $"ATK: {playerStats.ATK}";
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


        for (int i = 0; i < equipped; i++)
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

        for (int i = equipped; i < gearSlots.Count; i++)
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

    private void SwitchToConfirmSkipPanel()
    {
        miniInventory.GetChild(1).gameObject.SetActive(false);
        miniInventory.GetChild(3).gameObject.SetActive(true);
    }

    private void SwitchBackToGearPanel()
    {
        UpdateMiniInventory(playerID);
        miniInventory.GetChild(1).gameObject.SetActive(true);
        miniInventory.GetChild(2).gameObject.SetActive(false);
        miniInventory.GetChild(3).gameObject.SetActive(false);
    }

    private void ResetDisplays()
    {
        foreach (var display in itemDisplays)
        {
            Destroy(display.gameObject);
        }

        itemDisplays.Clear();
    }
}
