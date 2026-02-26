using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    //[Header("")]
    [SerializeField] private List<GameObject> playerList = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisablePlayerMovement(GameObject player)
    {
        if (player == null) return;
        player.GetComponent<PlayerMovement>().DisableMovement();
    }

    public void EnablePlayerMovement(GameObject player)
    {
        if (player == null) return;
        player.GetComponent <PlayerMovement>().EnableMovement();
    }

    public void DisableAllPlayerMovement()
    {
        foreach(var player in playerList)
        {
            if (player == null) continue;
            player.GetComponent<PlayerMovement>().DisableMovement();
        }
    }

    public void EnableAllPlayerMovement()
    {
        foreach (var player in playerList)
        {
            if (player == null) continue;
            player.GetComponent<PlayerMovement>().EnableMovement();
        }
    }
}
