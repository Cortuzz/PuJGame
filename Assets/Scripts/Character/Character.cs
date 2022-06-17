using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character: MonoBehaviour, ICharacter
{
    public int health;
    public int maxHealth;
    public int initialHealth;

    public float horizontalSpeed;

    public bool onGround = false;

    public Vector2 spawnPos;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;
    protected Collider _collider;
    protected Animator _animator;

    public abstract void SetOnGround(bool onGround);

    public abstract void Die();

    public abstract void CheckCollisions();

    public abstract void MoveUpdate();

    protected abstract Collider GetCollider();

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetCollider();
    }

    public Vector2 GetPosition()
    {
        return _rb.position;
    }

    public void Spawn()
    {
        health = initialHealth;
        GetComponent<Transform>().position = spawnPos;
    }

    public void Spawn(int x, int y)
    {
        spawnPos = new Vector2(x, y);
        Spawn();
    }

    public void CheckDirection()
    {
        Vector3 scale = transform.localScale;

        if (horizontalSpeed > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        }
        else if (horizontalSpeed < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        health -= damage;
        if (health < 0)
            Die();
    }
}