using UnityEngine;


public abstract class ScreenUI : MonoBehaviour
{
    public string screenName;
    public abstract void ShowScreenUI();

    public abstract void HideScreenUI();    
}
