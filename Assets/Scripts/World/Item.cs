using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item Class")]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int stackCount;
    [SerializeField] private Tile _placementBlock = null;

    public Block GetPlacementBlock()
    {
        if (_placementBlock == null)
            return null;

        return _placementBlock.CreateBlock();
    }
}
