using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Slime : Enemy
{
    private int _patrolCount = 0;
    public override void CheckCollisions()
    {
        bool collision = _collider.CheckBottomCollision();
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    protected override Collider GetCollider()
    {
        return new RectCollider(transform, rb, _spriteRenderer);
    }

    public override void MoveUpdate()
    {
        _patrolCount++;
        
        if (_patrolCount % 500 == 0)
        {
            _patrolCount = 0;
            horizontalSpeed *= -1;
        }

        rb.velocity = new(horizontalSpeed * 10, Mathf.Max(rb.velocity.y, -50));
    }

    public override void SetOnGround(bool onGround)
    {
        this.onGround = onGround;
    }

    protected sealed override void GiveDrop()
    {
        
    }

    void FixedUpdate()
    {
        CheckCollisions();
        CheckDirection();
        MoveUpdate();
    }
}
