using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    public float runSpeed = 1f;
    public float jumpSpeed = 1f;
    public float jumpTime = .3f;
    public float hangTime = .2f;
    public int jumpCount = 1;

    private bool facingRight = true;

    public bool grounded = false;
    public bool cielinged = false;
    private bool jumping = false;
    private float jumpClock = 0f;
    private float jumpDuration = 0f;

    private SpriteRenderer sprite;

    void Awake()
    {
        Vector3 spawnpos = GameObject.FindGameObjectWithTag("")
        transform.position = spawnpos;
    }

	// Use this for initialization
	void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    
    void FixedUpdate() {
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

        rb.velocity = new Vector2(runSpeed * x, rb.velocity.y);

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
