using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int playerID;
    private int slotIndex;

    public void Initialize(int playerID, int slotIndex)
    {
        this.playerID = playerID;
        this.slotIndex = slotIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // UIManager.Instance.OpenScreen("Item Info");
        ItemInfoUI ui = (ItemInfoUI)UIManager.Instance.GetScreen("Item Info");
        ui.DisplayItemInfo(slotIndex, playerID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // UIManager.Instance.CloseScreen("Item Info", false);
        ItemInfoUI ui = (ItemInfoUI)UIManager.Instance.GetScreen("Item Info");
        ui.HideScreenUI();
    }
}
