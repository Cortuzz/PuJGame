using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileClass", menuName = "Tile Class")]
public class Tile : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public float rarity;
    public Vector2 pos { get { return _pos; } set { _pos = value; } }
    private Vector2 _pos;

    public Tile prototype()
    {
        Tile newTile = new Tile();

        newTile.name = name;
        newTile.sprite = sprite;
        newTile.rarity = rarity;
        newTile._pos = _pos;

        return newTile;
    }
}
