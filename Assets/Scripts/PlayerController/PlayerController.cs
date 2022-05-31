using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float gravity;
    public float horizontalSpeed;
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

    private void OnTriggerEnter2D(Collider2D collision)
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

    // Update is called once per frame
    void Update()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalSpeed * speed, 0);
        CheckDirection();

        bool jumping = Input.GetKey(KeyCode.Space);
        if (jumping && onGround)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }
}
