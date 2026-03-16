using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    [Header("UI Screens")]
    [SerializeField] private List<ScreenUI> screens = new();

    [Header("UI Elements")]
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private GameObject itemDisplay;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void OpenScreen(string screen)
    {
        foreach(var s in screens)
        {
            if(screen == s.screenName)
            {
                Dim();
                s.ShowScreenUI();
            }
        }
    }

    public void CloseScreen(string screen)
    {
        foreach(var s in screens)
        {
            if(screen == s.screenName)
            {
                RemoveDim();
                s.HideScreenUI();
            }
        }
    }

    public ScreenUI GetScreen(string screen)
    {
        foreach(var s in screens)
        {
            if(screen == s.screenName)
            {
                return s;
            }
        }
        return null;
    }

    public void Dim()
    {
        dimPanel.SetActive(true);
    }

    public void RemoveDim()
    {
        dimPanel.SetActive(false);
    }

    public ItemDisplay CreateItemDisplay(Transform parentPanel, float scaleOffset)
    {
        GameObject display = Instantiate(itemDisplay);
        display.transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        display.transform.SetParent(parentPanel, false);
        display.SetActive(true);

        return display.GetComponent<ItemDisplay>();
    }

}
