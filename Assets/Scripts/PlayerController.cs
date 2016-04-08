using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    public float runSpeed = 1f;
    public float jumpSpeed = 1f;
    public float jumpTime = .3f;
    public float hangTime = .2f;
    private bool facingRight = true;

    public bool grounded = false;
    private bool jumping = false;
    private float jumpClock = 0f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void FixedUpdate() {
        if (!jumping)
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y - (jumpSpeed * hangTime), -jumpSpeed));
        else
        {
            if (Time.fixedTime - jumpClock > jumpTime)
                jumping = false;
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

        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;

        /*// Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale; */
    }
}
