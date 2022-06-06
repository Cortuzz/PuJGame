using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    public float speed;
    public float jumpForce;

    public float jumpTicks;
    public float jumpSpeed;
    public float maxJumpSpeed;
    public float holdJumpTime;
    public bool holdingJump = false;

    public override void SetOnGround(bool onGround)
    {
        if (onGround)
            jumpTicks = 0;
        this.onGround = onGround;
    }

    public override void Die()
    {
        Spawn();
    }

    protected override Collider GetCollider()
    {
        return new(transform, rb);
    }

    public override void CheckCollisions()
    {
        bool collision = _collider.CheckBottomCollision();
        if (collision)
            TakeDamage((int)(jumpTicks / 5 - 40)); // TODO: ѕроблема в более быстром / медленном падении
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    public bool CheckFall()
    {
        return rb.velocity.y < 0 && !onGround;
    }

    public override void MoveUpdate()
    {
        horizontalSpeed = Input.GetAxis("Horizontal");

        Vector2 movement = new(horizontalSpeed * speed, Mathf.Max(rb.velocity.y, -50));

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

    private void FixedUpdate()
    {
        CheckCollisions();
        CheckDirection();
        MoveUpdate();
    }
}
