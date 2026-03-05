using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Constellate/Gun")]
public class Gun : Item
{
    [SerializeField] private int damage;
    [SerializeField] private int numBullets;

    [Tooltip("Interval between each bullet")]
    [SerializeField] private float burstInterval;

    [Tooltip("Interval between bursts")]
    [SerializeField] private float fireInterval;

    [Tooltip("Visuals/Sprite of Bullets")]
    [SerializeField] private GameObject bulletPrefab;

    public int Damage => damage;
    public int NumBullets => numBullets;
    public float FireInterval => fireInterval;
    public float BurstInterval => burstInterval;
    public GameObject BulletPrefab => bulletPrefab;

}
