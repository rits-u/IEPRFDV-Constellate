using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void HealPlayer(PlayerStats p, int heal) 
    {
        //p.Health += heal;
        Debug.Log(p.name + "gained " + heal + " HP!");
    }
}
