using DentedPixel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerSprite : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("Weapon")]
    [SerializeField] private Transform visualWeapon;

    [Header("Properties")]
    [SerializeField] private float hoverDistance = 0.3f;
    [SerializeField] private float hoverDuration = 2;
    [SerializeField] private float leanAmount = 20;
    [SerializeField] private float leanTime = 0.5f;

    private SpriteRenderer spriteRenderer;
    private bool hovering;
    private Vector3 startPosition;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.localPosition;
    }

    private void Awake()
    {
        LeanTween.init(800);
    }

    public void UpdateVisual(float moveX)
    {
        if(moveX == 0)
        {
            if(!hovering) //start hovering
            {
                Hover();
            }
        }
        else
        {
            if(hovering)
            {
                LeanTween.cancel(gameObject);   //reset
                transform.localPosition = Vector3.zero;
                hovering = false;
            }

            Lean(moveX);
        }
    }

    private void Lean(float moveX)
    {
        float target = -moveX * leanAmount;
        LeanTween.rotateZ(gameObject, target, leanTime);
    }

    private void Hover()
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalY(gameObject, startPosition.y + hoverDistance, hoverDuration)
            .setLoopPingPong();

        LeanTween.rotateZ(gameObject, 0f, leanTime);
        hovering = true;
    }

    public void SwitchToLeft()
    {
        spriteRenderer.sprite = leftSprite;
        visualWeapon.localScale = new Vector3(1, 1, 1);
        visualWeapon.localPosition = new Vector3(-0.2f, -0.2f, 0f);
    }

    public void SwitchToRight()
    {
        spriteRenderer.sprite = rightSprite;
        visualWeapon.localScale = new Vector3(-1, 1, 1);
        visualWeapon.localPosition = new Vector3(0.2f, -0.2f, 0f);
    }

    //public void DashLean(float input)
    //{
    //    float leanAmount = 30f;
    //    float target = -input * leanAmount;
    //    LeanTween.cancel(gameObject);
    //    LeanTween.rotateZ(gameObject, target, 0.5f);
    //}

}
