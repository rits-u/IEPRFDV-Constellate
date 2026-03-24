using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ItemInfoUI : ScreenUI
{

    private ItemDisplay display;

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);
    }

    public override void HideScreenUI()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        display = UIManager.Instance.CreateItemDisplay(this.transform, 0.62f);
    }

    public void DisplayItemInfo(int index, int ID)
    {
        ShowScreenUI();
        transform.position = Input.mousePosition + new Vector3(-350, 0, 0);
        Gear gear = PlayerManager.Instance.AccessPlayerInventory(ID).GetEquippedGearByIndex(index).gear;
        display.DisplayEquippedGear(gear);


    }
 }
