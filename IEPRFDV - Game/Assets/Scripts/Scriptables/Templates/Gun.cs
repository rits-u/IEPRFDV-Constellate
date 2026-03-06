using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Gun", menuName = "Constellate/Gun")]
public class Gun : Item
{
    [SerializeField] private int damage;
    [SerializeField] private int numBullets;

    [Tooltip("Interval between each bullet")]
    [SerializeField] private float burstInterval;

    [Tooltip("Interval between bursts")]
    [SerializeField] private float fireInterval;

    [Tooltip("Visuals/Sprite of Bullets")]
    [SerializeField] private GameObject bulletPrefab;

    public int Damage
    {
        get => damage;
        set => damage = value;
    } 

    public int NumBullets
    {
        get => numBullets;
        set => numBullets = value;
    }

    public float FireInterval
    { 
        get => fireInterval;
        set => fireInterval = value;
    }
    public float BurstInterval
    { 
        get => burstInterval;
        set => burstInterval = value;
    }
    public GameObject BulletPrefab
    { 
        get => bulletPrefab;
        set => bulletPrefab = value;
    }

    //public override void UpgradeToNextTier()
    //{
    //    damage *= 2;
    //    numBullets += (Mathf.CeilToInt(numBullets / 2));
    //    //needs adjustment
    //}

    //public void Upgrade(Gun baseGun, int tier)
    //{
    //    damage = baseGun.damage * tier;
    //    numBullets += (Mathf.CeilToInt(baseGun.numBullets / 2));
    //}

    public void Upgrade(int tier)
    {
        damage = damage * tier;
        numBullets += (Mathf.CeilToInt(numBullets / 2));
    }

}
