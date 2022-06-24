using System;
using UnityEngine;

public sealed class Slime : Enemy
{
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
    private BoxCollider2D _boxCollider;

    private int deathTime = 80;
    private bool isDeath = false;

    private int damageTime = 100;
    private int initialDamage = 100;
    private int _collisionTimer = 4; 
    
    protected override void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Entity"))
            return;
        
        
        TakeDamage(col.GetComponent<Damage>().damage);
        Destroy(col.gameObject);
        damageTime = initialDamage;
        _animator.Play("GetDamage");
    }

    public override void CheckCollisions()
    {
        _collisionTimer -= 1;
        if (_collisionTimer > 0)
            return;

        _collisionTimer = 4;
        bool collision = _collider.CheckBottomCollision();
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    protected override Collider GetCollider()
    {
        RectCollider rectCollider = new RectCollider(transform, _rb, _boxCollider, true);
        //collider.SetBias(new Vector2(0.65f, -0.1f));
        return rectCollider;
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
            horizontalSpeed = Mathf.Abs(horizontalSpeed) * Mathf.Sign(World.player.GetPosition().x - _rb.position.x);
        }

        _rb.velocity = new(horizontalSpeed * 10, Mathf.Max(_rb.velocity.y, -50));
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
        if (isDeath || damageTime > 0)
            return;

        Vector2 playerPosition = World.player.GetPosition();
        Vector2 position = _rb.position;
        var distance =
            Mathf.Sqrt(Mathf.Pow(playerPosition.x - position.x, 2) + Mathf.Pow(playerPosition.y - position.y, 2));

        isAggred = distance < _triggerDistance;

        if (distance < 2)
        {
            _animator.Play("Attack");
            return;
        }

        _animator.Play("Idle");
    }

    void FixedUpdate()
    {
        var pos = transform.position;
        var playerPos = World.player.transform.position;
        var xDist = Mathf.Abs(pos.x - playerPos.x);
        var yDist = Mathf.Abs(pos.y - playerPos.y);
        
        if (xDist + yDist > 50)
        {
            World.mobCount--;
            Destroy(gameObject);
            return;
        }

        CheckCollisions();
        CheckDirection();
        MoveUpdate();
        CheckAggro();

        if (isDeath) 
            deathTime--;

        if (deathTime == 0)
            Destroy(gameObject);

        damageTime = Mathf.Max(damageTime - 1, 0);

    }

    public override void Die()
    {
        _animator.Play("Death");
        World.mobCount--;
        GiveDrop();
        isDeath = true;
    }
}