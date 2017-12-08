using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController2 : MonoBehaviour
{
    public float movespeed = 60;
    public float gravity = -200;
    public float jumpVelocity = 12;

    private Vector2 velocity;
    private bool facingRight = true;
    private SpriteRenderer sprite;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        HandleInput();

        // If the input is moving the player right and the player is facing left
        if (velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right
        else if (velocity.x < 0 && facingRight)
        {
            Flip();
        }
        //sends movement data to collision controller
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleInput()
    {
        //Get x-axis input
        float x = 0f;
        x = Input.GetAxis("Horizontal");
        //x = Mathf.Lerp(.5f, 1f, x);

        //Get y-axis input
        float y = 0f;
        y = Input.GetAxis("Vertical");

        //Get jump bool
        bool jump = (Input.GetAxis("Jump") != 0);

        if (jump)
        {
            velocity.y = jumpVelocity;
        }

        velocity.x = x * movespeed;
        velocity.y += gravity * Time.deltaTime;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        sprite.flipX = !sprite.flipX;
    }
}