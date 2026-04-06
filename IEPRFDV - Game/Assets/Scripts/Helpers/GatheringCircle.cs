using UnityEngine;

public class GatheringCircle : MonoBehaviour
{
    [Header("Circle Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite player1;
    [SerializeField] private Sprite player2;
    [SerializeField] private Sprite complete;

    private SpriteRenderer sr;
    int playersDetected = 0;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void PlayerEntered(Player player)
    {
        switch (player.ID)
        {
            case 1:
                sr.sprite = player1;
                break;
            case 2:
                sr.sprite = player2;
                break;
        }
    }

    private void PlayerExited(Player player)
    {
        switch (player.ID)
        {
            case 1:
                sr.sprite = player2;
                break;
            case 2:
                sr.sprite = player1;
                break;
        }
    }

    private void NoPlayers()
    {
        sr.sprite = defaultSprite;
    }

    private void CircleComplete()
    {
        sr.sprite = complete;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersDetected++;
            PlayerEntered(other.GetComponent<Player>());
        }

        if(playersDetected == 2)
        {
            CircleComplete();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersDetected--;
            PlayerExited(other.GetComponent<Player>());
        }

        if (playersDetected == 0)
        {
            NoPlayers();
        }
    }
}
