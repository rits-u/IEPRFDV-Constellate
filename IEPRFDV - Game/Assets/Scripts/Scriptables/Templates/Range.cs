using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "Range", menuName = "Constellate/Range")]
public class Range : Weapon
{
    [Header("Properties")]
    [Tooltip("Each element is by tier")]
    [SerializeField] private List<RangeProperties> rangeProperties = new();
    [Serializable]
    public class RangeProperties
    {
        [SerializeField] private int damage;
        [SerializeField] private int numProjectiles;
        [SerializeField] private float burstInterval;
        [SerializeField] private float fireRate;

        public int Damage => damage;
        public int NumProjectiles => numProjectiles;
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
                property = rangeProperties[CurrentTier - 1].Damage;
                break;
            case InfoType.PROJECTILES:
                property = rangeProperties[CurrentTier - 1].NumProjectiles;
                break;
            case InfoType.BURST_INTERVAL:
                property = rangeProperties[CurrentTier - 1].BurstInterval;
                break;
            case InfoType.FIRE_RATE:
                property = rangeProperties[CurrentTier - 1].FireRate;
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
                property = rangeProperties[tier - 1].Damage;
                break;
            case InfoType.PROJECTILES:
                property = rangeProperties[tier - 1].NumProjectiles;
                break;
            case InfoType.BURST_INTERVAL:
                property = rangeProperties[tier - 1].BurstInterval;
                break;
            case InfoType.FIRE_RATE:
                property = rangeProperties[tier - 1].FireRate;
                break;
        }

        return property;
    }
}
