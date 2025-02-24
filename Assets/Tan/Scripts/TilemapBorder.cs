using UnityEngine;

public class TilemapBorder : MonoBehaviour
{
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
}


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
