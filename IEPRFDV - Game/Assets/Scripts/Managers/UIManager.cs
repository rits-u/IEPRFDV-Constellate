using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Transform gameCanvas;

    [Header("UI Screens")]
    [SerializeField] private List<ScreenUI> screens = new();

    [Header("UI Elements")]
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private GameObject itemDisplay;

    [SerializeField] private List<GameObject> playerInfoHUDs;

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
                HideAllHUDs();
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
                ShowAllHUDs();
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

    public void ShowAllHUDs()
    {
        gameCanvas.gameObject.SetActive(true);
    }

    public void HideAllHUDs()
    {
        gameCanvas.gameObject.SetActive(false);
    }

    public void ShowPlayerInfoHUDs()
    {
        foreach(var s in playerInfoHUDs)
        {
            s.gameObject.SetActive(true);
        }
    }

    public void HidePlayerInfoHUDs()
    {
        foreach (var s in playerInfoHUDs)
        {
            s.gameObject.SetActive(false);
        }
    }

    public ItemDisplay CreateItemDisplay(Transform parentPanel, float scaleOffset)
    {
        GameObject display = Instantiate(itemDisplay);
        display.transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        display.transform.SetParent(parentPanel, false);
        display.SetActive(true);

        return display.GetComponent<ItemDisplay>();
    }

    public void DisableGameCanvas()
    {
        for(int i = 0; i < gameCanvas.childCount; i++)
        {
            gameCanvas.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void EnableGameCanvas()
    {
        for (int i = 0; i < gameCanvas.childCount; i++)
        {
            gameCanvas.GetChild(i).gameObject.SetActive(true);
        }
    }

}
