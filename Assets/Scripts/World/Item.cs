using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item Class")]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public Sprite additionalSprite;
    public int stackCount;
    public bool isWeapon;
    public bool isTool;
    [SerializeField] private Tile _placementBlock;

    public Block GetPlacementBlock()
    {
        if (_placementBlock == null)
            return null;

        return _placementBlock.CreateBlock();
    }
}
