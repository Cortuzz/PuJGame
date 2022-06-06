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
    public float minHeight;
    public bool isBackground = false;
    public Tile Prototype()
    {
        Tile newTile = CreateInstance<Tile>();

        newTile.name = name;
        newTile.sprite = sprite;
        newTile.renderBorder = renderBorder;
        newTile.maxHeight = maxHeight;
        newTile.minHeight = minHeight;
        newTile.isBackground = isBackground;

        return newTile;
    }

    public void SetRenderSettings(float renderBorder, float maxHeight, float minHeight)
    {
        this.renderBorder = renderBorder;
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
    }

    public Block CreateBlock()
    {
        return new Block(name, sprite, isBackground);
    }
}


public class Block
{
    public string name;
    public Sprite sprite;
    public bool isBackground;

    public Block(string name, Sprite sprite, bool isBackground)
    {
        this.name = name;
        this.sprite = sprite;
        this.isBackground = isBackground;
    }
}
