using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileClass", menuName = "Tile Class")]
public class Tile : ScriptableObject
{
    public Sprite sprite;
    public Item itemDrop;
    public new string name;
    public Color mapColor;
    public float renderBorder;
    public float maxHeight;
    public float minHeight;
    public bool isBackground = false;
    public Tile Prototype()
    {
        Tile newTile = CreateInstance<Tile>();
        newTile.itemDrop = itemDrop;
        newTile.name = name;
        newTile.sprite = sprite;
        newTile.renderBorder = renderBorder;
        newTile.maxHeight = maxHeight;
        newTile.minHeight = minHeight;
        newTile.isBackground = isBackground;
        newTile.mapColor = mapColor;

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
        return new Block(name, itemDrop, sprite,  isBackground, mapColor);
    }
}


public class Block
{
    public string name;
    public Item itemDrop;
    public Sprite sprite;
    public bool isBackground;
    public Color color;

    public Block(string name, Item itemDrop, Sprite sprite, bool isBackground, Color color)
    {
        this.name = name;
        this.itemDrop = itemDrop;
        this.sprite = sprite;
        this.isBackground = isBackground;
        this.color = color;
    }
}
