using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    public bool CheckBottomCollision();

    public void CheckTopCollision();

    public void CheckSideCollision();
}
