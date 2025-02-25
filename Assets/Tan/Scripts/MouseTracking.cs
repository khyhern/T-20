using UnityEngine;

public class MouseTracking : MonoBehaviour
{

    public Rigidbody2D rb;
    public Camera cam;             //Reference camera


    Vector2 mousePos;           //Store mouse position 


    // Update is called once per frame 
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Vector2 lookDir = mousePos - rb.position;  //look direction = mouse position - character current position 
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}
