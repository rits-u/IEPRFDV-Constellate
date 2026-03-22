using UnityEngine;

public class TitleUI : ScreenUI
{
    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);
    }

    public override void HideScreenUI()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        screenName = "Title";
    //    UIManager.Instance.HideAllHUDs();
    }
}
