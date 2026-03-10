using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Gun", menuName = "Constellate/Gun")]
public class Gun : Item
{
    //[SerializeField] private int damage;
    //[SerializeField] private int numBullets;

    //[Tooltip("Interval between each bullet")]
    //[SerializeField] private float burstInterval;

    //[Tooltip("Interval between bursts")]
    //[SerializeField] private float fireInterval;

    //[Tooltip("Visuals/Sprite of Bullets")]
    //[SerializeField] private GameObject bulletPrefab;

    //public int Damage => damage;

    //public int NumBullets => numBullets;

    //public float BurstInterval => burstInterval;

    //public float FireInterval => fireInterval;

    //public GameObject BulletPrefab => bulletPrefab;

    [Header("Properties")]
    [SerializeField] private int currentTier = 1;
    public int CurrentTier
    {
        get => currentTier;
        set => currentTier = value;
    }

    [SerializeField] private float rangeRadius = 10f;
    public float RangeRadius
    {
        get => rangeRadius;
        set => rangeRadius = value;
    }

    [Tooltip("Each element is by tier")]
    [SerializeField] private List<GunProperties> gunProperties = new();
    [Serializable]
    public class GunProperties
    {
        [SerializeField] private int damage;
        [SerializeField] private int numBullets;
        [SerializeField] private float burstInterval;
        [SerializeField] private float fireRate;

        public int Damage => damage;
        public int NumBullets => numBullets;
        public float BurstInterval => burstInterval;
        public float FireRate => fireRate;
    }

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    public GameObject BulletPrefab => bulletPrefab;

    public float GetPropertyByType(InfoType type)
    {
        float property = 0f;
        switch(type)
        {
            case InfoType.DAMAGE:   //int
                property = gunProperties[currentTier - 1].Damage;
                break;
            case InfoType.BULLETS:
                property = gunProperties[currentTier - 1].NumBullets;
                break;
            case InfoType.BURST_INTERVAL:
                property = gunProperties[currentTier - 1].BurstInterval;
                break;
            case InfoType.FIRE_RATE:
                property = gunProperties[currentTier - 1].FireRate;
                break;
        }

        return property;
    }

    public float GetProperty(InfoType type, int tier)
    {
        float property = 0f;
        switch(type)
        {
            case InfoType.DAMAGE:   //int
                property = gunProperties[tier - 1].Damage;
                break;
            case InfoType.BULLETS:
                property = gunProperties[tier - 1].NumBullets;
                break;
            case InfoType.BURST_INTERVAL:
                property = gunProperties[tier - 1].BurstInterval;
                break;
            case InfoType.FIRE_RATE:
                property = gunProperties[tier - 1].FireRate;
                break;
        }

        return property;
    }

    

    //public void Upgrade(int tier)
    //{
    //    damage = damage * tier;
    //    numBullets += (Mathf.CeilToInt(numBullets / 2));
    //}

}
