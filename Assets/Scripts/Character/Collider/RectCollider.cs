using System.Collections;
using System.Collections.Generic;
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

    public override Vector2 GetRoundedPosition(Transform _transform)
    {
        return new((int)_transform.position.x, (int)_transform.position.y);
    }

    private Vector2 GetLeftBottomPosition(Transform _transform, float biasY = 0, float biasX = 0)
    {
        return new((int)(_collider.bounds.center.x - _size.x / 2 + biasX), (int)(_collider.bounds.center.y - _size.y / 2 + biasY));
    }

    private Vector2 GetRigthTopPosition(Transform _transform, float biasY = 0, float biasX = 0)
    {
        return new((int)(_collider.bounds.center.x + _size.x / 2 - biasX), (int)(_collider.bounds.center.y + _size.y / 2 - biasY));
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

        Vector2 roundedPos = GetRigthTopPosition(_transform);
        int chunkNumber = GetChunkNumber(ref roundedPos, World.chunkSize);
        Block[,] chunk = World.blocks[chunkNumber];

        Block topBlock = chunk[(int)roundedPos.x, (int)roundedPos.y + 1];
        if (topBlock != null && _transform.position.y > roundedPos.y + 0.2f)
        {
            _transform.position = new(_transform.position.x, roundedPos.y);
            _rb.velocity = new(_rb.velocity.x, Mathf.Min(0, _rb.velocity.y));
        }
    }

    public override void CheckSideCollision()
    {
        Vector2 roundedLeftPos = GetLeftBottomPosition(_transform, 0.01f);
        Vector2 roundedRigthPos = GetRigthTopPosition(_transform);
        int chunkNumberLeft = GetChunkNumber(ref roundedLeftPos, World.chunkSize);
        int chunkNumberRigth = GetChunkNumber(ref roundedRigthPos, World.chunkSize);

        Block[,] chunkLeft = World.blocks[chunkNumberLeft];
        Block[,] chunkRigth = World.blocks[chunkNumberRigth];
        Block bottomLeftBlock = chunkLeft[(int)roundedLeftPos.x, (int)roundedLeftPos.y];
        Block bottomRigthBlock = chunkRigth[(int)roundedRigthPos.x, (int)roundedLeftPos.y];

        Block topLeftBlock = chunkLeft[(int)roundedLeftPos.x, (int)roundedRigthPos.y];
        Block topRigthBlock = chunkRigth[(int)roundedRigthPos.x, (int)roundedRigthPos.y];

        bool isLeftCollision = bottomLeftBlock != null || topLeftBlock != null;
        bool isRightCollision = bottomRigthBlock != null || topRigthBlock != null;
        float xPos;

        if (!isLeftCollision && !isRightCollision)
            return;
        else if (isRightCollision && isLeftCollision)
        {
            xPos = _transform.position.x; // TODO
        }
        else if (isRightCollision)
        {
            xPos = roundedLeftPos.x + chunkNumberLeft * World.chunkSize;
        }
        else
        {
            xPos = roundedRigthPos.x + chunkNumberRigth * World.chunkSize - _size.x;
        }

        _transform.position = new(xPos, _transform.position.y);
    }
}
