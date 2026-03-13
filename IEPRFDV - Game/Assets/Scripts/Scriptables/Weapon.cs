using UnityEngine;

public abstract class Weapon : Item
{
    [Header("Weapon Fields")]
    [SerializeField] private int currentTier = 1;
    [SerializeField] private float rangeRadius;
    [SerializeField] private bool isMelee;

    public int CurrentTier
    {
        get => currentTier;
        set => currentTier = value;
    }

    public float RangeRadius => rangeRadius;

    public bool IsMelee => isMelee;
}
