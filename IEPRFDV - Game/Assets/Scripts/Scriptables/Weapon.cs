using UnityEngine;

public abstract class Weapon : Item
{
    [Header("Weapon Fields")]
    [SerializeField] private int currentTier = 1;
    [SerializeField] private float rangeRadius;
    [SerializeField] private bool isMelee;

    [Header("SFX")]
    [SerializeField] private AudioClip[] listSFX;


    public int CurrentTier
    {
        get => currentTier;
        set => currentTier = value;
    }

    public float RangeRadius => rangeRadius;

    public bool IsMelee => isMelee;

    public AudioClip SFX()
    {
        int index = Random.Range(0, listSFX.Length);
        return listSFX[index];
    }
}
