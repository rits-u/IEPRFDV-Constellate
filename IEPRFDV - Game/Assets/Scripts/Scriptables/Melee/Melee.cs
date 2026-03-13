using System;
using System.Collections.Generic;
using UnityEngine;
using static Gun;


[CreateAssetMenu(fileName = "Melee", menuName = "Constellate/Melee")]
public class Melee : ScriptableObject
{
    [Header("Properties")]
    [SerializeField] private int currentTier = 1;
    public int CurrentTier
    {
        get => currentTier;
        set => currentTier = value;
    }

    [SerializeField] private float rangeRadius = 5f;
    public float RangeRadius
    {
        get => rangeRadius;
        set => rangeRadius = value;
    }


    [Tooltip("Each element is by tier")]
    [SerializeField] private List<MeleeProperties> meleeProperties = new();
    [Serializable]
    public class MeleeProperties
    {
        [SerializeField] private int damage;
        [SerializeField] private float slashInterval;

        public int Damage => damage;
        public float SlashInterval => slashInterval;
    }

    [Header("VFX")]
    [SerializeField] private GameObject slashPrefab;
    public GameObject SlashPrefab => slashPrefab;

    public float GetPropertyByType(InfoType type)
    {
        float property = 0f;
        switch (type)
        {
            case InfoType.DAMAGE:   //int
                property = meleeProperties[currentTier - 1].Damage;
                break;
            case InfoType.SLASH_INTERVAL:
                property = meleeProperties[currentTier - 1].SlashInterval;
                break;
        }

        return property;
    }


    public float GetProperty(InfoType type, int tier)
    {
        float property = 0f;
        switch (type)
        {
            case InfoType.DAMAGE:   //int
                property = meleeProperties[tier - 1].Damage;
                break;
            case InfoType.SLASH_INTERVAL:
                property = meleeProperties[tier - 1].SlashInterval;
                break;
        }

        return property;
    }
}
