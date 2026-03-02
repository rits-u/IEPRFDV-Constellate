using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private int maxSlots;
    [SerializeField] private List<Gear> gearList = new();

    //[Header("UI Elements")]
    private Stats playerStats;


    private void Start()
    {
        playerStats = GetComponent<Stats>();
    }
    public void EquipGear(Gear gear)
    {
        playerStats.HP += gear.HP;
        playerStats.ATK += gear.ATK;
        playerStats.SP += gear.SP;
        
        gearList.Add(gear);

    }

    public void UnEquipGear(Gear gear)
    {
        playerStats.HP -= gear.HP;
        playerStats.ATK -= gear.ATK;
        playerStats.SP -= gear.SP;

        int index = 0;
        foreach (var g in gearList)
        {
            if (g == gear) break;
            index++;
        }

        gearList.RemoveAt(index);


    }
}
