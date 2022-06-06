using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected abstract void GiveDrop();
    public override void Die()
    {
        GiveDrop();
        Destroy(gameObject);
    }
}
