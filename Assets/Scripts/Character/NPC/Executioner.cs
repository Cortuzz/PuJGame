using System;
using UnityEngine;

public class Executioner : Enemy
{
    public int weaponDamage;
    public GameObject summonPrefab;
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
    private BoxCollider2D _boxCollider;

    private int _followingTimer = 9500;
    private int _initFollowingTimer = 10000;
    private float _savedPlayerPos;

    private int spawnSummonTime;

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

    private void SpawnSummons(Vector3 position)
    {
        var summon = Instantiate(summonPrefab, transform.parent.parent);
        summon.transform.position = transform.position + position;
    }

    private void FollowToPlayer()
    {
        var difPos = World.player.transform.position - transform.position;

        _rb.velocity = _followingTimer switch
        {
            > 9700 => new Vector2(50 * _savedPlayerPos, 0),
            > 9000 => new Vector2(Mathf.Sign(difPos.x) / 5, Mathf.Sign(difPos.y) / 5),
            < 200 => new Vector2(Mathf.Sign(difPos.x) / 5, 5 * difPos.y),
            < 500 => new Vector2(-15 * Mathf.Sign(difPos.x), 5 * difPos.y),
            < 6000 and >= 4000 => (_followingTimer % 200 < 100) ? new Vector2(difPos.x, difPos.y + 15) : new Vector2(difPos.x, difPos.y + 10) ,
            _ => difPos / 2
        };

        if (_followingTimer is 5500 or 4500 or 5000)
        {
                SpawnSummons(new Vector2(3, 0));
                spawnSummonTime = 175;
        }

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

        if (spawnSummonTime > 0)
        {
            _animator.Play("Summon");
            return;
        }

        if (!isAggred || _followingTimer > 9850)
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
        spawnSummonTime = Mathf.Max(spawnSummonTime - 1, 0);
    }
}