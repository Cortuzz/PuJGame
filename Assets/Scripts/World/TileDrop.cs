using UnityEngine;

public class TileDrop : MonoBehaviour
{
    public Item item;
    private bool _instantiated = false;
    private BoxCollider2D _boxCollider;
    private RectCollider _collider;
    private Rigidbody2D _rb;

    public void Instantiate()
    {
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = item.sprite;
        _collider = new RectCollider(transform, _rb, _boxCollider, true);
        _instantiated = true;
    }

    public void CheckCollisions()
    {
        _collider.CheckBottomCollision();
    }

    private void FixedUpdate()
    {
        if (!_instantiated)
            return;

        CheckCollisions();
    }
}
