using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    [SerializeField] private WeightedRandomList<Item> lootTable;
 //   [SerializeField] private List<WeightedRandomList.Pair> lootList = new();

    [SerializeField] private int numLoot;

    [SerializeField] private GameObject displayPrefab;

    [Header("UI Elements")]
    [SerializeField] private GameObject upperPanel;

    private int lootWeaponAmount;

    public void RandomizeLoot()
    {
      //  numLoot = Random.Range(1, 2);
        numLoot = 2;
        lootWeaponAmount = 0;

        for (int i = 0; i < numLoot; i++)
        {
            var item = lootTable.GetRandom();

            //check if it gets weapon again
            if(item.type == LootType.WEAPON && lootWeaponAmount != 0)
            {
                i--;
                continue;
            }


            //itemdisplay prefab -> display stats and effect

            switch (item.type)
            {
                case LootType.WEAPON:
                {
                    Gun gun = (Gun)item.item;
                    ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
                    itemDisplay.EditNameTextBox(gun.itemName);

                    itemDisplay.EditTextBoxByIndex(gun.Damage.ToString(), 0);
                    itemDisplay.SetIconByIndex(InfoType.DAMAGE, 0);

                    itemDisplay.EditTextBoxByIndex(gun.NumBullets.ToString(), 1);
                    itemDisplay.SetIconByIndex(InfoType.BULLETS, 1);
                        // EditTextBoxByIndex
                    lootWeaponAmount++;


                    lootTable.Remove(item); //remove from the drops
                    break;
                }
                case LootType.GEAR:
                {
                    Gear gear = (Gear)item.item;
                    ItemDisplay itemDisplay = InstantiateDisplay().GetComponent<ItemDisplay>();
                    itemDisplay.EditNameTextBox(gear.itemName);

                    Dictionary<InfoType, int> stats = new();
                    if (gear.HP != 0) stats.Add(InfoType.HP, gear.HP);
                    if (gear.ATK != 0) stats.Add(InfoType.ATK, gear.ATK);
                    if (gear.SP != 0) stats.Add(InfoType.SP, gear.SP);

                    int index = 0;

                    foreach (var stat in stats)
                    {
                        itemDisplay.EditTextBoxByIndex(stat.Value.ToString(), index);
                        itemDisplay.SetIconByIndex(stat.Key, index);
                        index++;
                    }


                    break;
                }
            }            
        }
    }

    private GameObject InstantiateDisplay()
    {
        GameObject display = Instantiate(displayPrefab);
        display.transform.localScale = Vector3.one;
        display.transform.SetParent(upperPanel.transform, false);
        display.SetActive(true);

        return display;
    }
}
