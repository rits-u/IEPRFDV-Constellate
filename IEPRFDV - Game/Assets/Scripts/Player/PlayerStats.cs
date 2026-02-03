using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    [SerializeField] private float HP;
    [SerializeField] private float ATK;

    public float Health
    {
        get => HP;
        set => HP = value;
    }
}
