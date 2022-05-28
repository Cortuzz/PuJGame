using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorDirector : MonoBehaviour
{
    public TileAtlas atlas;

    public int width = 500;
    public int height = 400;

    public int seed;
    public float caveFreq = 0.08f;
    public float terrainFreq = 0.04f;

    void Start()
    {
        seed = Random.Range(-1000, 1000);
        //seed = 1000;
        WorldGeneration worldGenerator = new(width, height, seed, terrainFreq);
        
        worldGenerator.GenerateTile(worldGenerator.GenerateNoiseTexture(caveFreq), atlas.stone);
        worldGenerator.GenerateTile(worldGenerator.GenerateNoiseTexture(0.08f), atlas.dirt);
        GenerateOres(worldGenerator);

        worldGenerator.GenerateTerrain(atlas.dirt);
        worldGenerator.GenerateTopBlocks(atlas.grass);
        worldGenerator.RemoveTopBlocksExcept(atlas.grass);
        
        RenderTiles(worldGenerator.GetTiles());
    }

    public void GenerateOres(WorldGeneration worldGenerator)
    {
        Tile[] ores = new Tile[3] { atlas.coal, atlas.iron, atlas.diamond };

        foreach (Tile ore in ores)
        {
            worldGenerator.GenerateTile(worldGenerator.GenerateNoiseTexture(0.08f), ore);
        }
    }

    public void RenderTiles(Tile[,] tiles)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                RenderTile(tiles[j, i], j, i);
            }
        }
    }

    private void RenderTile(Tile tile, int x, int y)
    {
        if (tile == null)
            return;

        GameObject tileObject = new(name = tile.name);
        tileObject.transform.parent = this.transform;
        tileObject.AddComponent<SpriteRenderer>();
        tileObject.GetComponent<SpriteRenderer>().sprite = tile.sprite;

        tileObject.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}
