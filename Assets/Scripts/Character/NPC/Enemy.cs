using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character.Character
{
    public bool isAggred = false;
    public int bodyDamage = 15;

    protected abstract void GiveDrop();

    protected abstract void CheckAggro();

    public override void Die()
    {
        GiveDrop();
        Destroy(gameObject);
    }
}
