using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public Tile dirt;
    public Tile stone;
}
