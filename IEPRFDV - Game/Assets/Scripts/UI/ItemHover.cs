using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverDuration = 0.25f;

    private int playerID;
    private int slotIndex;

    Coroutine hoverRoutine;

    public void Initialize(int playerID, int slotIndex)
    {
        this.playerID = playerID;
        this.slotIndex = slotIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverRoutine = StartCoroutine(HoverToItem());
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(hoverRoutine);

        ItemInfoUI ui = (ItemInfoUI)UIManager.Instance.GetScreen("Item Info");
        ui.HideScreenUI();
    }

    private IEnumerator HoverToItem()
    {
        yield return new WaitForSeconds(hoverDuration);

        ItemInfoUI ui = (ItemInfoUI)UIManager.Instance.GetScreen("Item Info");
        ui.DisplayItemInfo(slotIndex, playerID);
    }
}
