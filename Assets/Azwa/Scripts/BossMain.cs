using UnityEngine;

public class BossMain : MonoBehaviour
{

    //Idle 
    [Header("Idle")]
    [SerializeField] float idleMovementSpeed;
    [SerializeField] Vector2 idleMovementDirection;

    //Slam
    [Header("Slam")]
    [SerializeField] float slamMovementSpeed;
    [SerializeField] Vector2 slamMovementDirection;

    //Charge 
    [Header("ChargePlayer")]
    [SerializeField] float chargePlayerSpeed;
    [SerializeField] Transform player;
    private Vector2 playerPosition;
    private bool hasPlayerPosition;

    //Function
    [Header("Other")]
    [SerializeField] Transform GroundCheckUp;
    [SerializeField] Transform GroundCheckDown;
    [SerializeField] Transform GroundCheckWall;
    [SerializeField] float GroundCheckRadius;
    [SerializeField] LayerMask GroundLayer;
    private bool isTouchingUp;
    private bool isTouchingDown;
    private bool isTouchingWall;

    private bool goingUp = true;
    private bool facingLeft = true; 
    private Rigidbody2D enemyRB;

    private Animator enemyAni;
    void Start()
    {
        idleMovementDirection.Normalize();
        slamMovementDirection.Normalize();
        enemyRB= GetComponent<Rigidbody2D>();
        enemyAni= GetComponent<Animator>();
    }

  
    void Update()
    {
       isTouchingUp = Physics2D.OverlapCircle(GroundCheckUp.position, GroundCheckRadius, GroundLayer);
       isTouchingDown = Physics2D.OverlapCircle(GroundCheckDown.position, GroundCheckRadius, GroundLayer);
       isTouchingWall = Physics2D.OverlapCircle(GroundCheckWall.position, GroundCheckRadius, GroundLayer);
        //Idle();
        //Slam();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChargePlayer();
        }
    }

    void randomStatePicker()
    {
        int randomState = Random.Range(0, 2);
        if (randomState == 0)
        {
            enemyAni.SetTrigger("Slam");
        }
        else if (randomState == 1)
        {
            enemyAni.SetTrigger("Charge");
        }
    }

    public void Idle()
    {
        if (isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if (isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if(isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        enemyRB.linearVelocity = idleMovementSpeed * idleMovementDirection;
    }

    public void Slam()
    {
        if (isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if (isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if (isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        enemyRB.linearVelocity = slamMovementSpeed * slamMovementDirection;
    }

    public void ChargePlayer()
    {
        if (!hasPlayerPosition)
        {
            playerPosition = player.position - transform.position;
            playerPosition.Normalize();
            hasPlayerPosition = true;
        }
        if (hasPlayerPosition)
        {
            enemyRB.linearVelocity = playerPosition * chargePlayerSpeed;
        }
        if( isTouchingWall || isTouchingDown)
        {
            enemyRB.linearVelocity = Vector2.zero;
            hasPlayerPosition = false;
            enemyAni.SetTrigger("Hit");
        }
    }

    void FlipToPlayer()
    {
        float playerDirection = player.position.x - transform.position.x;

        if (playerDirection > 0 && facingLeft)
        {
            Flip();
        }
        else if (playerDirection < 0 && !facingLeft)
        {
            Flip();
        }
    }

    void ChangeDirection()
    {
        goingUp = !goingUp;
        idleMovementDirection.y *= -1;
        slamMovementDirection.y *= -1;
    }

    void Flip()
    {
        facingLeft = !facingLeft;
        idleMovementDirection.x *= -1;
        slamMovementDirection.x *= -1;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GroundCheckUp.position, GroundCheckRadius);
        Gizmos.DrawWireSphere(GroundCheckDown.position, GroundCheckRadius);
        Gizmos.DrawWireSphere(GroundCheckWall.position, GroundCheckRadius);
    }
}
