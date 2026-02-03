using UnityEngine;

public class HealOrb : Item
{
    [SerializeField] private int heal = 10;

    public override void ActivateItem(PlayerStats p)
    {
        PlayerManager.Instance.HealPlayer(p, heal);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Debug.Log("helloooooo");

        if (other.CompareTag("Player"))
        {
            if (this.Type == ItemType.Orb)
            {
                ActivateItem(other.GetComponent<PlayerStats>());
            }
        }
    }
}
