using System;
using UnityEngine;

public class Summon : Enemy
{
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
    private BoxCollider2D _boxCollider;

    private int _followingTimer;
    private int _initFollowingTimer = 1000;
    private float _savedPlayerPos;
    private int _spawnTimer = 50; 

    protected override void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        base.Awake();
        _animator.Play("Spawn");
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
        var difPos = World.player.transform.position - transform.position;

        _rb.velocity = _followingTimer switch
        {
            > 850 => new Vector2(-Mathf.Sign(difPos.x) / 2, Mathf.Sign(difPos.y)),
            > 150 => new Vector2(10 * Mathf.Sign(difPos.x), 10 * Mathf.Sign(difPos.y)),
            _ => new Vector2(Mathf.Sign(difPos.x), -Mathf.Sign(difPos.y) / 2)
        };
        

        if (_followingTimer > 0)
            return;

        _savedPlayerPos = Mathf.Sign(difPos.x);
        _followingTimer = _initFollowingTimer;
    }

    public override void SetOnGround(bool onGround)
    {
        this.onGround = onGround;
    }

    private void FollowToPlayer()
    {

    }

    protected override void GiveDrop()
    {
        
    }

    protected override void CheckAggro()
    {
        Vector2 playerPosition = World.player.GetPosition();
        Vector2 position = _rb.position;
        isAggred = Mathf.Sqrt(Mathf.Pow(playerPosition.x - position.x, 2) + Mathf.Pow(playerPosition.y - position.y, 2)) < _triggerDistance;
    }

    void FixedUpdate()
    {
        _spawnTimer = Mathf.Max(_spawnTimer - 1, 0);
        if (_spawnTimer > 0)
            return;
        
        CheckDirection();
        MoveUpdate();
        CheckAggro();
        _animator.Play("Idle");
        _followingTimer = Mathf.Max(_followingTimer - 1, 0);
    }
}