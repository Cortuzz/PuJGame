using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    public bool isAggred = false;
    protected abstract void GiveDrop();

    protected abstract void CheckAggro();

    public override void Die()
    {
        GiveDrop();
        Destroy(gameObject);
    }
}
