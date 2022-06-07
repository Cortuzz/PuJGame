using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Slime : Enemy
{
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
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

        if (isAggred)
        {
            horizontalSpeed = Mathf.Abs(horizontalSpeed) * Mathf.Sign(World.player.GetPosition().x - rb.position.x);
        }

        rb.velocity = new(horizontalSpeed * 10, Mathf.Max(rb.velocity.y, -50));
    }

    public override void SetOnGround(bool onGround)
    {
        this.onGround = onGround;
    }

    protected override void GiveDrop()
    {
        
    }

    protected override void CheckAggro()
    {
        Vector2 playerPosition = World.player.GetPosition();
        Vector2 position = rb.position;
        isAggred = Mathf.Sqrt(Mathf.Pow(playerPosition.x - position.x, 2) + Mathf.Pow(playerPosition.y - position.y, 2)) < _triggerDistance;
    }

    void FixedUpdate()
    {
        CheckCollisions();
        CheckDirection();
        MoveUpdate();
        CheckAggro();
    }


}
