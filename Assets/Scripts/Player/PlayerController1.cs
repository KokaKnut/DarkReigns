using UnityEngine;
using System.Collections;

public class PlayerController1 : MonoBehaviour
{

    public enum STATE
    {
        idle,
        walking,
        jumping,
        roping,
        shooting,
        dead
    }

    [SerializeField]
    private STATE _state = STATE.idle;
    public STATE state
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            gameObject.layer = 8; // 8 is Player
            switch (value)
            {
                case STATE.idle:
                    animator.SetInteger("animState", 0);
                    break;
                case STATE.walking:
                    animator.SetInteger("animState", 1);
                    break;
                case STATE.jumping:
                    animator.SetInteger("animState", 2);
                    break;
                case STATE.roping:
                    animator.SetInteger("animState", 2);
                    gameObject.layer = 12; // 12 is PlayerIgnoreWorld
                    break;
            }
        }
    }

    // TWEAK VARS---------------------------
    [Header("TWEAK VARS")]
    public float runSpeed = 1f;
    public float jumpSpeed = 1f;
    public float fallingSpeed = 1f;
    public float jumpTime = .3f;
    public float jumpDurationSlowdown = 20f;
    public float hangTimeUp = .2f;
    public float hangTimeDown = .2f;
    public float hangTimeDownTimeFactor = .2f;
    public int numberOfJumps = 1;
    public float fallFloorTime = .1f;

    // JUMP STUFF---------------------------
    [Header("VISIBLE VARS")]
    public bool grounded = false;
    public bool cielinged = false;
    public bool jumpReleased = true;
    public bool jumping = false;
    public float jumpClock = 0f;
    public float jumpDuration = 0f;
    public int jumpCount = 0;
    
    public GameObject rope;

    private bool facingRight = true;
    private SpriteRenderer sprite;
    private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = GameObject.FindGameObjectWithTag("World").GetComponent<TileMapGenerator>().SpawnPos();
    }

    void UpdateState()
    {
        if (state == STATE.roping)
        {
            if (rope == null || jumping)
            {
                state = STATE.jumping;
            }
        }

        if (state == STATE.jumping)
        {
            if (grounded)
            {
                state = STATE.walking;
            }
        }

        if (state == STATE.walking)
        {
            if (rb.velocity.y != 0f)
            {
                state = STATE.jumping;
            }
            else
            {
                if (rb.velocity.x == 0f)
                {
                    state = STATE.idle;
                }
            }
        }

        if (state == STATE.idle)
        {
            if (rb.velocity.x != 0f)
            {
                if (rb.velocity.y != 0f)
                {
                    state = STATE.jumping;
                }
                else
                {
                    state = STATE.walking;
                }
            }
            else
            {
                if (rb.velocity.y != 0f)
                {
                    state = STATE.jumping;
                }
            }
        }
    }

    void FixedUpdate()
    {
        UpdateState();

        if (state != STATE.roping)
        {
            if (!jumping)
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y - (jumpSpeed * hangTimeUp), -jumpSpeed));
                else
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y - (jumpSpeed * hangTimeDown * ((jumpDuration + jumpTime) / (jumpTime + hangTimeDownTimeFactor))), -fallingSpeed));
            }
            else
            {
                if (jumpDuration > jumpTime || cielinged)
                {
                    jumping = false;
                }
            }

            if (grounded || state != STATE.jumping)
            {
                jumpCount = 1;
                jumpDuration = jumpTime;
            }
        }
        else
        {
            jumpCount = 0;
            jumpDuration = jumpTime;
        }
    }

    public void Move(float x, float y, bool jump)
    {
        if (y < -.7f)
            FallFloor();

        //checking if we should grab a rope
        if (state != STATE.roping && y > .7f && rope != null)
        {
            state = STATE.roping;
            transform.position = new Vector3(rope.transform.position.x, transform.position.y, 0);
        }
        
        //setting up an initial speed
        rb.velocity = new Vector2(runSpeed * x, rb.velocity.y);

        //when the player has released the jump button for the first time in their jump
        if (!jump && jumpReleased == false)
        {
            jumpReleased = true;
            jumping = false;
            jumpCount++;

            //stops weird math from too short of jumps
            if (jumpDuration < jumpTime / 4)
                jumpDuration = jumpTime / 4;
        }

        //check if the player can jump and is trying to jump
        if (jump && jumpReleased && (grounded || numberOfJumps > jumpCount))
        {
            jumping = true;
            jumpReleased = false;
            jumpClock = Time.fixedTime;
            jumpDuration = 0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        //check if the player is in the middle of a jump
        if (jump && jumping)
        {
            jumpDuration = Time.fixedTime - jumpClock;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed - (jumpDuration * jumpDurationSlowdown));
        }

        //if the player is roping, don't let him move sideways
        if (state == STATE.roping)
        {
            rb.velocity = new Vector2(0, runSpeed * y);
        }

        // If the input is moving the player right and the player is facing left
        if (x > 0 && !facingRight)
        {
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right
        else if (x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void FallFloor()
    {
        //Collider2D[] fallFloors = Physics2D.OverlapAreaAll(rb.worldCenterOfMass - GetComponent<BoxCollider2D>().size, rb.worldCenterOfMass + GetComponent<BoxCollider2D>().size, 13);

        //foreach (Collider2D fallFloor in fallFloors)
        //
        //    Invoke("UndoFallFloor", fallFloorTime);
        //    
        //}

        Physics2D.IgnoreLayerCollision(8, 13, true);
        Invoke("UndoFallFloor", fallFloorTime);
    }

    private void UndoFallFloor()
    {
        Physics2D.IgnoreLayerCollision(8, 13, false);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        sprite.flipX = !sprite.flipX;
    }
}
