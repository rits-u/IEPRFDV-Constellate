using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Gear", menuName = "Constellate/Gear")]
public class Gear : Item
{
    [Header("Properties")]
    [SerializeField] private int currentTier = 1;
    public int CurrentTier
    {
        get => currentTier;
        set => currentTier = value;
    }

    [SerializeField] private int expiration;
    public int Expiration
    {
        get => expiration;
        set => expiration = value;
    }


    [Tooltip("Each element of the list is by tier")]
    public List<GearStats> gearStats = new();
    [Serializable]
    public class GearStats
    {
        [SerializeField] private int health;
        [SerializeField] private int attack;
        [SerializeField] private int shield;
        

        public int HP => health;
        public int ATK => attack;
        public int SP => shield;
    }

    [Header("If a stat is 0, leave it uncheck")]
    public bool hasHPStat;
    public bool hasATKStat;
    public bool hasSPStat;


    [Header("Buff")]
    public string effect;

    
    public int GetStatsByType(InfoType type)
    {
        int stat = 0;
        switch (type)
        {
            case InfoType.HP: stat = gearStats[currentTier - 1].HP; break;
            case InfoType.ATK: stat = gearStats[currentTier - 1].ATK; break;
            case InfoType.SP: stat = gearStats[currentTier - 1].SP; break;
        }
        return stat;
    }

    public int GetStats(InfoType type, int tier)
    {
        int stat = 0;
        switch(type)
        {
            case InfoType.HP: stat = gearStats[tier-1].HP; break;
            case InfoType.ATK: stat = gearStats[tier - 1].ATK; break;
            case InfoType.SP: stat = gearStats[tier - 1].SP; break;
        }
        return stat;
    }


}