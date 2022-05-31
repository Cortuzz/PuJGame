using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBiomeFactory
{
    Tile GetTerrainTile();

    Tile GetTopTerrainTile();

    Tile GetCaveTile();
}
