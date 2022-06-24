using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private RectCollider _collider;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = new RectCollider(transform, _rb, GetComponent<BoxCollider2D>());
    }
    
    void FixedUpdate()
    {
        var velocity = _rb.velocity;
        var xVel = velocity.x - Mathf.Sign(velocity.x) * 0.01;
        var angle = Quaternion.LookRotation(Vector3.forward, velocity).eulerAngles.z;

        _rb.velocity = new Vector2((float)xVel, _rb.velocity.y);
        transform.localRotation = Quaternion.Euler(0, 0, angle - 20);
        
        if (_collider.CheckBottomCollision() || _collider.CheckSideCollision())
            Destroy(gameObject);
    }
}
