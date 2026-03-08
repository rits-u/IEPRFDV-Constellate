using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Constellate/Heal")]
public class Heal : Item
{
    [Header("Stats")]
    [SerializeField] private int healAmount;


    [Header("Info")]
    //  public int tier;
    public string description;

    public int HealAmount => healAmount;

}