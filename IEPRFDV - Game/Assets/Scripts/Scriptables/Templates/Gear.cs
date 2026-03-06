using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "Constellate/Gear")]
public class Gear : Item
{
    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private int attack;
    [SerializeField] private int shield;


    [Header("Info")]
    //  public int tier;
    public string effect;

    public int HP => health;
    public int ATK => attack;
    public int SP => shield;

    //public override void UpgradeToNextTier()
    //{
    //    health *= 2;
    //    attack *= 2;
    //    shield *= 2;
    //    //needs adjustment
    //}

    //public void Upgrade(Gear gear, int tier)
    //{

    //}



}