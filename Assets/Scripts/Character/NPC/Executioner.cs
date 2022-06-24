using System;
using UnityEngine;

public class Executioner : Enemy
{
    public int weaponDamage;
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
    private BoxCollider2D _boxCollider;

    private int _followingTimer;
    private int _initFollowingTimer = 2000;
    private float _savedPlayerPos;

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
    }

    public override void CheckCollisions()
    {
    }

    protected override Collider GetCollider()
    {
        return null;
    }

    public override void MoveUpdate()
    {
        if (_followingTimer > 500)
            return;
        
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

        var distanceY = World.player.transform.position.y - transform.position.y;
        _rb.velocity = new(horizontalSpeed * 10, Mathf.Max(distanceY));
    }

    public override void SetOnGround(bool onGround)
    {
        this.onGround = onGround;
    }

    protected override void GiveDrop()
    {
    }

    private void FollowToPlayer()
    {
        var difPos = World.player.transform.position - transform.position;

        _rb.velocity = _followingTimer switch
        {
            > 1920 => new Vector2(100 * _savedPlayerPos, 0),
            < 200 => new Vector2(Mathf.Sign(difPos.x), 5 * difPos.y),
            < 400 => new Vector2(-20 * Mathf.Sign(difPos.x), 5 * difPos.y),
            _ => difPos
        };

        if (_followingTimer > 0)
            return;

        _savedPlayerPos = Mathf.Sign(difPos.x);
        _followingTimer = _initFollowingTimer;
    }

    protected override void CheckAggro()
    {
        Vector2 playerPosition = World.player.GetPosition();
        Vector2 position = _rb.position;
        isAggred = Mathf.Sqrt(Mathf.Pow(playerPosition.x - position.x, 2) + Mathf.Pow(playerPosition.y - position.y, 2)) < _triggerDistance;

        if (!isAggred || _followingTimer > 1920)
        {
            _animator.Play("Idle");
            return;
        }

        _animator.Play("Attack");
    }

    void FixedUpdate()
    {
        CheckDirection();
        MoveUpdate();
        FollowToPlayer();
        CheckAggro();
        _followingTimer = Mathf.Max(_followingTimer - 1, 0);
    }
}