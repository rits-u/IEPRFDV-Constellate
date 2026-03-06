using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private int playerIndex;

    [Header("UI Elements")]
    [Header("Weapon")]
    [SerializeField] private TextMeshProUGUI dmgTextbox;
    [SerializeField] private TextMeshProUGUI numBulletsTextbox;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI hpTextbox;
    [SerializeField] private TextMeshProUGUI atkTextbox;
    [SerializeField] private TextMeshProUGUI spTextbox;

    private GameObject player;

    private void Start()
    {
        player = PlayerManager.Instance.GetPlayerByIndex(playerIndex-1);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        dmgTextbox.text = "DMG: " + inventory.GetPlayerGun().Damage;
        numBulletsTextbox.text = "BU: " + inventory.GetPlayerGun().NumBullets;

        Stats playerStats = player.GetComponent<Stats>();
        hpTextbox.text = "HP: " + playerStats.HP;
        atkTextbox.text = "ATK: " + playerStats.ATK;
        spTextbox.text = "SP: " + playerStats.SP;
    }
}
