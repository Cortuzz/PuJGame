using UnityEngine;

public sealed class Slime : Enemy
{
    private int _patrolCount = 0;
    private int _triggerDistance = 5;
    private BoxCollider2D _boxCollider;

    protected override void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        base.Awake();
    }

    public override void CheckCollisions()
    {
        bool collision = _collider.CheckBottomCollision();
        SetOnGround(collision);

        _collider.CheckTopCollision();
        _collider.CheckSideCollision();
    }

    protected override Collider GetCollider()
    {
        RectCollider rectCollider = new RectCollider(transform, _rb, _boxCollider);
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
        Vector2 playerPosition = World.player.GetPosition();
        Vector2 position = _rb.position;
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
