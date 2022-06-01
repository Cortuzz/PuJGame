using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float gravity;

    public float horizontalSpeed;
    public float jumpSpeed;
    public bool onGround = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = gravity;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
        
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    private void CheckDirection()
    {
        if (horizontalSpeed > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        } 
        else if (horizontalSpeed < 0) 
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");
        jumpSpeed = Input.GetAxisRaw("Jump");

        Vector2 movement = new(horizontalSpeed * speed, rb.velocity.y);
        CheckDirection();

        if (jumpSpeed > 0.1f && onGround)
        {
            movement.y = jumpForce * jumpSpeed;
        }

        rb.velocity = movement;
    }
}
