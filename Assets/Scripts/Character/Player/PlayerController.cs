using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICharacter
{
    public int health;
    public int maxHealth = 200;
    public int initialHealth = 100;

    public float speed;
    public float jumpForce;

    public float horizontalSpeed;

    public float jumpTicks;
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
        health = initialHealth;
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
        if (onGround)
            jumpTicks = 0;
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

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        health -= damage;
        if (health < 0)
            Spawn();
    }

    public void CheckCollisions()
    {
        bool collision = _collider.CheckBottomCollision();
        if (collision)
            TakeDamage((int)(jumpTicks / 10 - 40)); // ѕроблема в более быстром / медленном падении
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    public bool CheckFall()
    {
        return rb.velocity.y < 0 && !onGround;
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

        if (CheckFall())
            ++jumpTicks;
        else
            jumpTicks = 0;
            
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
