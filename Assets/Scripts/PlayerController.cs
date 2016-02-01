using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    public float runSpeed;
    private bool facingRight = true;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void FixedUpdate() {
        
    }

    public void Move(float h, float v, bool jump)
    {
        if (jump)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 1);

        rb.velocity = new Vector2(runSpeed * h, rb.velocity.y);

        // If the input is moving the player right and the player is facing left...
        if (h > 0 && !facingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (h < 0 && facingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
