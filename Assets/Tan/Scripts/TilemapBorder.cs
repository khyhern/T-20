using UnityEngine;

public class TilemapBorder : MonoBehaviour
{
    private Collider2D myCollider;
    private GameObject player;
    //private bool playerExited = true; // Track if player has exited before re-enabling collision

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        // If player is to the left, set isTrigger true immediately
        if (player.transform.position.x < transform.position.x)
        {
            myCollider.isTrigger = true;
            //playerExited = false; // Player has entered, so wait before turning collision back on
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            //playerExited = true; // Player has fully exited
            myCollider.isTrigger = false; // Re-enable solid collision
        }
    }
}


/*{
    private Collider2D myCollider;
    private GameObject player;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        // Compare player position with the border position
        if (player.transform.position.x > transform.position.x)
        {
            // Player is to the RIGHT → Collider is solid
            myCollider.isTrigger = false;
        }
        else
        {
            // Player is to the LEFT → Collider is a trigger
            myCollider.isTrigger = true;
        }
    }
}*/


/*{
    private Collider2D myCollider;
    private GameObject player;
    private float lastX; // Store last frame's X position

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            lastX = player.transform.position.x;
        }
    }

    void Update()
    {
        if (player == null) return;

        float currentX = player.transform.position.x;

        if (currentX > lastX)
        {
            // Player moving RIGHT → Keep trigger ON
            myCollider.isTrigger = true;
        }
        else if (currentX < lastX)
        {
            // Player moving LEFT → Turn OFF trigger
            myCollider.isTrigger = false;
        }

        lastX = currentX; // Update lastX for next frame
    }
}*/
