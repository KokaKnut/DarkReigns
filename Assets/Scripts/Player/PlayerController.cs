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
    
    public STATE state = STATE.idle;
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

    void Awake()
    {
        state = STATE.idle;
    }

	// Use this for initialization
	void Start() {
        gameObject.transform.position = GameObject.FindGameObjectWithTag("World").GetComponent<TileMapGenerator>().SpawnPos();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    
    void FixedUpdate() {
        if (state == STATE.roping && rope == null)
        {
            state = STATE.jumping;
        }

        if(state == STATE.jumping && grounded)
        {
            state = STATE.walking;
        }

        if(state == STATE.walking && rb.velocity.x == 0f)
        {
            state = STATE.idle;
        }

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
