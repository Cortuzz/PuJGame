using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSlot : MonoBehaviour
{
    public int index;

    public void Craft()
    {
        World.craftController.Craft(index);
    }
}
