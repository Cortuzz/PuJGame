using UnityEngine;

public class RectCollider : Collider
{
    private Vector2 _size;
    private BoxCollider2D _collider;
    private bool _isTile;

    public RectCollider(Transform tr, Rigidbody2D rb, BoxCollider2D collider, bool isTile = false) : base(tr, rb)
    {
        _size = collider.size * tr.localScale;
        _size.x = Mathf.Abs(_size.x);
        _collider = collider;
        _isTile = isTile;
    }

    public override Vector2 GetRoundedPosition(Transform transform)
    {
        var position = transform.position;
        return new((int)position.x, (int)position.y);
    }

    private Vector2 GetLeftBottomPosition(Transform transform, float biasY = 0, float biasX = 0)
    {
        var bounds = _collider.bounds;
        return new((int)(bounds.center.x - _size.x / 2 + biasX),
            (int)(bounds.center.y - _size.y / 2 + biasY));
    }

    private Vector2 GetRigthTopPosition(Transform transform, float biasY = 0, float biasX = 0)
    {
        var bounds = _collider.bounds;
        return new((int)(bounds.center.x + _size.x / 2 - biasX),
            (int)(bounds.center.y + _size.y / 2 - biasY));
    }

    public override bool CheckBottomCollision()
    {
        Vector2 roundedLeftPos = GetLeftBottomPosition(_transform, biasX: 0.05f);
        Vector2 roundedRigthPos = GetRigthTopPosition(_transform, biasX: 0.05f);
        int chunkNumberLeft = GetChunkNumber(ref roundedLeftPos, World.chunkSize);
        int chunkNumberRigth = GetChunkNumber(ref roundedRigthPos, World.chunkSize);

        Block[,] chunkLeft = World.blocks[chunkNumberLeft];
        Block[,] chunkRigth = World.blocks[chunkNumberRigth];
        Block bottomLeftBlock = chunkLeft[(int)roundedLeftPos.x, (int)roundedLeftPos.y];
        Block bottomRigthBlock = chunkRigth[(int)roundedRigthPos.x, (int)roundedLeftPos.y];

        bool isBlockCollision = bottomLeftBlock != null || bottomRigthBlock != null;

        bool isCollision = isBlockCollision && _rb.velocity.y <= 0;

        if (!isCollision)
            return false;

        var b = (_isTile) ? 0.5f : 1f;

        _transform.position = new(_transform.position.x, roundedRigthPos.y + b * _size.y);
        _rb.velocity = new(_rb.velocity.x, 0);
        return true;
    }

    public override void CheckTopCollision()
    {
        return; // TODO

        /*Vector2 roundedPos = GetRightTopPosition(_transform);
    int chunkNumber = GetChunkNumber(ref roundedPos, World.chunkSize);
    Block[,] chunk = World.blocks[chunkNumber];

    Block topBlock = chunk[(int)roundedPos.x, (int)roundedPos.y + 1];
    if (topBlock != null && _transform.position.y > roundedPos.y + 0.2f)
    {
        _transform.position = new(_transform.position.x, roundedPos.y);
        _rb.velocity = new(_rb.velocity.x, Mathf.Min(0, _rb.velocity.y));
    }*/
    }

    public override void CheckSideCollision()
    {
        var roundedLeftPos = GetLeftBottomPosition(_transform, 0.01f);
        var roundedRightPos = GetRigthTopPosition(_transform);
        var chunkNumberLeft = GetChunkNumber(ref roundedLeftPos, World.chunkSize);
        var chunkNumberRight = GetChunkNumber(ref roundedRightPos, World.chunkSize);

        var chunkLeft = World.blocks[chunkNumberLeft];
        var chunkRight = World.blocks[chunkNumberRight];
        var bottomLeftBlock = chunkLeft[(int)roundedLeftPos.x, (int)roundedLeftPos.y];
        var bottomRightBlock = chunkRight[(int)roundedRightPos.x, (int)roundedLeftPos.y];

        var topLeftBlock = chunkLeft[(int)roundedLeftPos.x, (int)roundedRightPos.y];
        var topRightBlock = chunkRight[(int)roundedRightPos.x, (int)roundedRightPos.y];

        var isLeftCollision = bottomLeftBlock != null || topLeftBlock != null;
        var isRightCollision = bottomRightBlock != null || topRightBlock != null;

        if (!isLeftCollision && !isRightCollision)
            return;

        var position = _transform.position;
        var xPos = isRightCollision switch
        {
            true when isLeftCollision => position.x,
            true => roundedRightPos.x + chunkNumberRight * World.chunkSize - _size.x / 2,
            _ => roundedRightPos.x + chunkNumberRight * World.chunkSize - 0.2f // todo: че за хуйня
        };

        position = new Vector3(Mathf.Lerp(position.x, xPos, 0.2f), position.y);
        _transform.position = position;
    }
}