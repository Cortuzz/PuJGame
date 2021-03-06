using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Basic tiles")]
    public Tile dirt;
    public Tile stone;
    public Tile grass;
    public Tile sand;

    [Header("Tree tiles")]
    public Tile trunkBottom;
    public Tile trunkMid;
    public Tile leaf;
    
    [Header("Ore tiles")]
    public Tile coal;
    public Tile iron;
    public Tile diamond;

    [Header("Other tiles")]
    public Tile air;
}
