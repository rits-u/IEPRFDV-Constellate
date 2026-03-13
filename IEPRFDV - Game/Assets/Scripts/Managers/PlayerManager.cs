using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    //[Header("")]
    [SerializeField] private List<GameObject> playerList = new();
    [SerializeField] private List<Transform> offsets = new();

    private int playersAlive;

    //singleton
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

    private void Start()
    {
        //foreach (var player in playerList)
        //{
        //    offsets.Add(player.GetComponentInChildren<OffsetTransform>().transform);
        //    Debug.Log(player.GetComponentInChildren<OffsetTransform>().transform.position);
        //}
        InitializePlayers();
    }

    public List<Transform> GetTransformList()
    {
        return offsets;
    }

    public void InitializePlayers()
    {
        foreach (var player in playerList)
        {
            Stats playerStats = player.GetComponent<Stats>();
            playerStats.OnDeath += UnregisterPlayer;
            playersAlive++;
        }
    }

    private void UnregisterPlayer(Stats playerStats)
    {
        playerStats.OnDeath -= UnregisterPlayer;
        int index = 0;
        foreach(var player in playerList)
        {
            if(player == playerStats.gameObject)
            {
                Debug.Log($"PM: {player.name} was defeated!");
                //DAWG //spawn resurrect circle stuff here hhkhsgkh
                //playersAlive--;
                break;
            }
            index++;
        }

        playerList.RemoveAt(index);
    }

    public GameObject GetPlayerByIndex(int index)
    {
        return playerList[index];
    }

    public void DisablePlayerMovement(GameObject player)
    {
        if (player == null) return;
        player.GetComponent<PlayerMovement>().DisableMovement();
    }

    public void EnablePlayerMovement(GameObject player)
    {
        if (player == null) return;
        player.GetComponent<PlayerMovement>().EnableMovement();
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

    public void AddPointToPlayer(GameObject player, int points)
    {
        foreach(var p in playerList)
        {
            if(player == p)
            {
                PlayerScore ps = p.GetComponent<PlayerScore>();
                if (ps != null)
                {
                    ps.Score += points;
                    ps.UpdateScoreUI();
                    // Debug.Log($"player score: {ps.Score}");
                    break;
                }
                else
                    Debug.Log("PlayerScore is Missing in " + p.name);
            }

        }
    }

    public void ApplyGearToPlayer(Gear gear, int playerID, int tier)
    {
        PlayerInventory inventory = playerList[playerID - 1].GetComponent<PlayerInventory>();
        if (inventory.GetEquippedGearCount() < inventory.GetMaxSlots())
        {
            inventory.EquipGear(gear, tier);
        }
        else
        {
            //let it discard// 
            Debug.Log($"LM: {gameObject.name}'s inventory is already FULL");
        }
    }

    public void SwitchWeaponOfPlayer(Weapon weapon, int playerID, int tier)
    {
        PlayerInventory inventory = playerList[playerID - 1].GetComponent<PlayerInventory>();
        inventory.EquipWeapon(weapon, tier);
    }

    public void ApplyHealToPlayer(Heal heal, int playerID, int tier)
    {
        Stats stats = playerList[playerID - 1].GetComponent<Stats>();
        stats.Heal(heal.HealAmount * tier);
    }

    public bool ArePlayersAlive()
    {
        if (playersAlive <= 1) return false;
        return true;
    }
}
