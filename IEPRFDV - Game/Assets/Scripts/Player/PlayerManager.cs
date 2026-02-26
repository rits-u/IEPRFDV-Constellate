using UnityEngine;

public class PlayerManagerOld : MonoBehaviour
{

    public static PlayerManagerOld Instance { get; private set; }
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

    public void HealPlayer(Stats p, int heal) 
    {
        //p.Health += heal;
        Debug.Log(p.name + "gained " + heal + " HP!");
    }
}
