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
    protected Rigidbody2D rb;
    protected SpriteRenderer _spriteRenderer;
    protected Collider _collider;

    public abstract void SetOnGround(bool onGround);
    public abstract void Die();
    public abstract void CheckCollisions();
    public abstract void MoveUpdate();

    protected abstract Collider GetCollider();

    public void Spawn()
    {
        health = initialHealth;
        rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetCollider();

        GetComponent<Transform>().position = spawnPos;
    }

    public void Spawn(int x, int y)
    {
        spawnPos = new Vector2(x, y);
        Spawn();
    }

    public void CheckDirection()
    {
        if (horizontalSpeed > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontalSpeed < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
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