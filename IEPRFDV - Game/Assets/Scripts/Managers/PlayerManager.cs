using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private List<Player> playerList = new();
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
            playerStats.OnDeath += OnPlayerDown;
            playersAlive++;
        }
        playerList[1].GetComponentInChildren<PlayerSprite>().SwitchToLeft();
    }

    private void OnDisable()
    {
        //foreach(var player in playerList)
        //{
        //    Stats playerStats = player.GetComponent<Stats>();
        //    playerStats.OnDeath -= OnPlayerDown;
        //    playersAlive--;
        //}
    }

    private void OnPlayerDown(Stats playerStats)
    {
        PlayerScore pScore = playerStats.GetComponent<PlayerScore>();
        pScore.RoundStreak = 1;     //resets
        playersAlive--;
        CheckPlayersCondition();
    }

    private void CheckPlayersCondition()
    {
        if(playersAlive == 0)
        {
            //stop the car, show results screen
            DisableAllPlayerMovement();
            foreach (var player in playerList)
            {
                player.GetComponentInChildren<ResurrectCircle>().
                    GetComponent<SpriteRenderer>().enabled = false;
                var ps = player.GetComponentInChildren<PlayerSprite>();
                if (ps != null) ps.DeathEffect();


            }

            RoundManager.Instance.StopRound();
        }
    }

    public void IncrementRoundStreak()
    {
        foreach(var player in playerList)
        {
            player.GetComponent<PlayerScore>().RoundStreak += 1;
        }
    }

    public Player GetPlayerByID(int ID)
    {
        foreach (var player in playerList)
        {
            if(player.ID == ID ) return player;
        }

        return null;
    }

    public void DisablePlayerMovement(Player player)
    {
        if (player == null) return;
        player.GetComponent<PlayerMovement>().DisableMovement();
    }

    public void EnablePlayerMovement(Player player)
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
            if(player == p.gameObject)
            {
                PlayerScore ps = p.GetComponent<PlayerScore>();
                if (ps != null)
                {
                    ps.CalculateScore(points);

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
        PlayerInventory inventory = GetPlayerByID(playerID).GetComponent<PlayerInventory>();
        if (inventory.GetEquippedGearCount() < inventory.GetMaxSlots())
        {
            inventory.EquipGear(gear, tier);
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

    public Weapon GetPlayerWeapon(int ID)
    {
        Weapon weapon = playerList[ID - 1].GetComponent<PlayerInventory>().GetPlayerWeapon();
        return weapon;
    }

    public PlayerInventory AccessPlayerInventory(int ID)
    {
        PlayerInventory inventory = playerList[ID - 1].GetComponent<PlayerInventory>();
        return inventory;
    }

    public void PutPlayerToDownState(Player player)
    {
        DisablePlayerMovement(player);
        AccessPlayerInventory(player.ID).DisableWeapon();

        player.GetComponentInChildren<DamageFlash>().enabled = false;
        player.GetComponent<Stats>().isDown = true;
    }

    public void PutPlayerToActiveState(Player player)
    {
        EnablePlayerMovement(player);
        AccessPlayerInventory(player.ID).EnableWeapon();

        playersAlive++;
        player.GetComponentInChildren<DamageFlash>().enabled = true;
        player.GetComponent<Stats>().isDown = false;
    }

    public Player DetermineWinner()
    {
        PlayerScore p1Score = playerList[0].GetComponent<PlayerScore>();
        PlayerScore p2Score = playerList[1].GetComponent<PlayerScore>();

        if (p1Score.Score == p2Score.Score) return null;

        return p1Score.Score > p2Score.Score ? playerList[0] : playerList[1];
    }

    public int GetPlayerScoreByID(int ID)
    {
        return playerList[ID - 1].GetComponent<PlayerScore>().Score;
    }

    public void CheckAllPlayersGears()
    {
        foreach (Player player in playerList)
        {
            player.GetComponent<PlayerInventory>().UpdateGearExpirations();
        }

    }
}
