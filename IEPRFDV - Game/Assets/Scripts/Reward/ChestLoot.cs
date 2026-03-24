using System.Collections.Generic;
using System;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    [Header("Loot Properties")]
    [SerializeField] private int maxLoot = 3;
    [SerializeField] private WeightedRandomList<Item> lootTable;

    [Header("UI Elements")]
    [SerializeField] private GameObject upperPanel;

    private List<ItemDisplay> itemDisplays = new();

    public void RandomizeLoot()
    {
        int numLoot = UnityEngine.Random.Range(1, maxLoot+1);
        //int numLoot = 3;

        int lootWeaponAmount = 0;
        int lootHeal = 0;
        ResetDisplay();

     

        for (int i = 0; i < numLoot; i++)
        {
            var item = lootTable.GetRandom();

            //check if it gets weapon/heal again
            if((item.type == ItemType.WEAPON && lootWeaponAmount != 0) ||
                item.type == ItemType.HEAL && lootHeal != 0)
            {
                i--;
                continue;
            }

            switch (item.type)
            {
                case ItemType.WEAPON:
                    lootWeaponAmount++;
                   // lootTable.Remove(item); //remove from the drops
                    break;
                
                case ItemType.GEAR:
                    break;

                case ItemType.HEAL:
                    lootHeal++;
                    break;
            }

            ItemDisplay display = UIManager.Instance.CreateItemDisplay(upperPanel.transform, 0.75f);
            itemDisplays.Add(display);
            display.Setup(item.item);

            LootManager.Instance.AddItemToLoot(item.item);
        }
    }

    private void ResetDisplay()
    {
//        Debug.Log("Displays: " + itemDisplays.Count);
        foreach(var display in itemDisplays)
        {
            Destroy(display.gameObject);
        }

        itemDisplays.Clear();
    }
}

