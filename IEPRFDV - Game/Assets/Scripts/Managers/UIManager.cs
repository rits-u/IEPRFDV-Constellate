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

    private void Awake()
    {
        if (Instance == null) Instance = this;
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

    private void Dim()
    {
        dimPanel.SetActive(true);
    }

    private void RemoveDim()
    {
        dimPanel.SetActive(false);
    }
}
