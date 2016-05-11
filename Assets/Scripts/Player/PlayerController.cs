using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

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
            }
        }
    }
    public GameObject rope;

    // TWEAK VARS---------------------------
    private Rigidbody2D rb;
    public float runSpeed = 1f;
    public float jumpSpeed = 1f;
    public float jumpTime = .3f;
    public float hangTime = .2f;
    public int jumpCount = 1;
    
    // JUMP STUFF---------------------------
    public bool grounded = false;
    public bool cielinged = false;
    private bool jumping = false;
    private float jumpClock = 0f;
    private float jumpDuration = 0f;
    
    private bool facingRight = true;
    private SpriteRenderer sprite;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

	// Use this for initialization
	void Start() {
        gameObject.transform.position = GameObject.FindGameObjectWithTag("World").GetComponent<TileMapGenerator>().SpawnPos();
    }
    
    void FixedUpdate() {
        UpdateState();
        
        if (!jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y - (jumpSpeed * hangTime), -jumpSpeed));
            jumpDuration = 0f;
        }
        else
        {
            if (jumpDuration > jumpTime || cielinged)
            {
                jumpDuration = 0f;
                jumping = false;
            }
        }
    }

    void UpdateState()
    {
        if (state == STATE.roping)
        {
            if (rope == null)
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

    public void Move(float x, float y, bool jump)
    {
        if (state != STATE.roping)
        {
            rb.velocity = new Vector2(runSpeed * x, rb.velocity.y);

            if (jump && grounded)
            {
                jumping = true;
                jumpClock = Time.fixedTime;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
            if (jump && jumping)
            {
                jumpDuration += Time.fixedTime - jumpClock;
                jumpClock = Time.fixedTime;
            }
            if (!jump)
            {
                jumping = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, runSpeed * y);
        }
        
        // If the input is moving the player right and the player is facing left...
        if (x > 0 && !facingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (x < 0 && facingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        sprite.flipX = !sprite.flipX;
    }
}
