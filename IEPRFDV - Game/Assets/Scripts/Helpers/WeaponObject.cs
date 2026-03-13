using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    public Weapon weapon;

    [SerializeField] private SpriteRenderer rangeRenderer;
    [SerializeField] private SpriteRenderer meleeRenderer;


    public void SetWeaponSprite(bool IsMelee, Sprite sprite)
    {
        if (IsMelee)
        {
            meleeRenderer.sprite = sprite;
            rangeRenderer.sprite = null;
        }
        else
        {
            rangeRenderer.sprite = sprite;
            meleeRenderer.sprite = null;
        }
    }

    public void SwitchToRange()
    {
        GetComponent<BasicMeleeBehavior>().enabled = false;
        GetComponent<BasicRangeBehavior>().enabled = true;
    }

    public void SwitchToMelee()
    {
        GetComponent<BasicRangeBehavior>().enabled = false;
        GetComponent<BasicMeleeBehavior>().enabled = true;
    }

}
