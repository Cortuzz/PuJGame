using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacter
{
    public float speed;
    public float jumpForce;

    public float horizontalSpeed;

    public float jumpSpeed;
    public float maxJumpSpeed;
    public float holdJumpTime;
    public bool holdingJump = false;
    public bool onGround = false;

    public Vector2 spawnPos;

    public Rigidbody2D rb;
    private SpriteRenderer _spriteRenderer;
    private Collider _collider;

    public void Spawn()
    {
        rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = new(transform, rb);

        GetComponent<Transform>().position = spawnPos;
    }

    public void Spawn(int x, int y)
    {
        spawnPos = new Vector2(x, y);
        Spawn();
    }

    public void SetOnGround(bool onGround)
    {
        this.onGround = onGround;
    }

    public void CheckDirection()
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

    public void CheckCollisions()
    {
        SetOnGround(_collider.CheckBottomCollision());
        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    public void MoveUpdate()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");

        Vector2 movement = new(horizontalSpeed * speed, Mathf.Max(rb.velocity.y, -50));
        CheckDirection();

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space) && holdingJump == false)
            {
                holdJumpTime = Time.time;
                holdingJump = true;
            }
                
            if (Input.GetKeyUp(KeyCode.Space) && holdingJump)
            {
                holdingJump = false;
                SetOnGround(false);
                movement.y = Mathf.Clamp(jumpForce * (Time.time - holdJumpTime), 5, maxJumpSpeed);
            }
        }

        rb.velocity = movement;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void FixedUpdate()
    {
        CheckCollisions();
        MoveUpdate();
    }
}
