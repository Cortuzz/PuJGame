using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileClass", menuName = "Tile Class")]
public class Tile : ScriptableObject
{
    public Sprite sprite;
    public new string name;
    public float renderBorder;
    public float maxHeight;
    public Tile Prototype()
    {
        Tile newTile = CreateInstance<Tile>();

        newTile.name = name;
        newTile.sprite = sprite;
        newTile.renderBorder = renderBorder;

        return newTile;
    }
}
