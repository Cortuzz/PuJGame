using UnityEngine;

public class Collider : ICollidable
{
    protected readonly Transform _transform;
    protected readonly Rigidbody2D _rb;
    protected Vector2 _bias;

    public Collider(Transform tr, Rigidbody2D rb)
    {
        _transform = tr;
        _rb = rb;
        _bias = Vector2.zero;
    }

    public void SetBias(Vector2 bias)
    {
        _bias = bias;
    }

    public virtual Vector2 GetRoundedPosition(Transform transform)
    {
        return new(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y));
    }

    public int GetChunkNumber(ref Vector2 roundedPos, int chunkSize)
    {
        int chunkNumber = (int)roundedPos.x / chunkSize;
        roundedPos.x %= chunkSize;
        return chunkNumber;
    }

    public virtual bool CheckBottomCollision()
    {
        Vector2 roundedPos = GetRoundedPosition(_transform);
        int chunkNumber = GetChunkNumber(ref roundedPos, World.chunkSize);

        Block[,] chunk = World.blocks[chunkNumber];
        Block bottomBlock = chunk[(int)roundedPos.x, (int)roundedPos.y - 2];
        /*Block leftBottomBlock;
        Block rightBottomBlock;

        try
        {
            rightBottomBlock = chunk[(int)roundedPos.x + 1, (int)roundedPos.y - 2];
        }
        catch
        {
            rightBottomBlock = World.blocks[chunkNumber + 1][0, (int)roundedPos.y - 2];
        }

        try
        {
            leftBottomBlock = chunk[(int)roundedPos.x - 1, (int)roundedPos.y - 2];
        }
        catch
        {
            leftBottomBlock = World.blocks[chunkNumber - 1][World.chunkSize - 1, (int)roundedPos.y - 2];
        }*/

        bool isBlockCollision = bottomBlock != null;
        //|| rightBottomBlock != null && Mathf.Abs(GetRoundedPosition(_transform).x - _transform.position.x) < 0.5f || 
        //leftBottomBlock != null;

        bool isCollision = isBlockCollision && _rb.velocity.y <= 0 && _transform.position.y < roundedPos.y - 0.2f;

        if (!isCollision)
            return false;

        int yPos;
        for (yPos = (int)roundedPos.y - 2; yPos < World.height; yPos++)
        {
            if (chunk[(int)roundedPos.x, yPos] == null)
            {
                roundedPos.y = yPos + 1;
                break;
            }
        }

        _transform.position = new(_transform.position.x, roundedPos.y - 0.2f);
        _rb.velocity = new(_rb.velocity.x, 0);
        return true;
    }

    public virtual void CheckTopCollision()
    {
        Vector2 roundedPos = GetRoundedPosition(_transform);
        int chunkNumber = GetChunkNumber(ref roundedPos, World.chunkSize);
        Block[,] chunk = World.blocks[chunkNumber];

        Block topBlock = chunk[(int)roundedPos.x, (int)roundedPos.y + 1];
        if (topBlock != null && _transform.position.y > roundedPos.y + 0.2f)
        {
            _transform.position = new(_transform.position.x, roundedPos.y + 0.2f);
            _rb.velocity = new(_rb.velocity.x, Mathf.Min(0, _rb.velocity.y));
        }
    }

    public virtual void CheckSideCollision()
    {
        Vector2 roundedPos = GetRoundedPosition(_transform);
        int chunkNumber = GetChunkNumber(ref roundedPos, World.chunkSize);
        Block[,] chunk = World.blocks[chunkNumber];

        for (int y = (int)roundedPos.y - 1; y <= (int)roundedPos.y; y++)
        {
            Block rightBlock;
            Block leftBlock;

            try
            {
                rightBlock = chunk[(int)roundedPos.x + 1, y];
            }
            catch
            {
                rightBlock = World.blocks[chunkNumber + 1][0, y];
            }

            try
            {
                leftBlock = chunk[(int)roundedPos.x - 1, y];
            }
            catch
            {
                leftBlock = World.blocks[chunkNumber - 1][World.chunkSize - 1, y];
            }

            if (rightBlock == null && leftBlock == null)
                continue;

            float xPos = roundedPos.x + World.chunkSize * chunkNumber;

            if (leftBlock != null && rightBlock != null)
                xPos += 0.5f;
            else if (leftBlock != null)
                xPos = Mathf.Lerp(_transform.position.x, Mathf.Max(_transform.position.x, xPos + 0.5f), 0.2f);
            else if (rightBlock != null)
                xPos = Mathf.Lerp(_transform.position.x, Mathf.Min(_transform.position.x, xPos + 0.5f), 0.2f);

            var newPos = new Vector2(xPos, _transform.position.y);
            _transform.position = newPos;
        }
    }
}