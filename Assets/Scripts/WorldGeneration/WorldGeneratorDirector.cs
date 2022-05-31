using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorDirector : MonoBehaviour
{
    //public IBiomeFactory biomeFactory = new ForestBiomeFactory();
    public TileAtlas atlas;
    public WorldGeneratorController controller;

    public int height = 400;
    public int width;
    public int chunkSize = 64;
    public int chunksCount = 2;

    public int seed;
    public float caveFreq = 0.08f;
    public float terrainFreq = 0.04f;

    private WorldGeneration _worldGenerator;

    private const int _oresCount = 3;

    void Start()
    {
        seed = Random.Range(-1000, 1000);
        //-185 -9 -545 814 (big hills)
        //seed = 814;
        width = chunksCount * chunkSize;
        controller = new(seed, width, height);

        GenerateChunks();
    }

    public void GenerateChunks()
    {
        for (int chunk = 0; chunk < chunksCount; chunk++)
        {
            GameObject chunkObject = new();
            chunkObject.name = "CHUNK " + (chunk + 1);
            chunkObject.transform.parent = this.transform;

            _worldGenerator = new(chunkSize, height, seed, terrainFreq);
            GenerateChunk(chunk * chunkSize);
            //RenderTiles(_worldGenerator.GetBlocks(), chunkObject, chunk * chunkSize);
        }
    }

    public void GenerateChunk(int bias)
    {
        GenerateCaves(bias);
        GenerateDirt(bias);
        GenerateOres(bias);
        GenerateTerrain();
        GenerateTunnels(bias);
    }

    public void GenerateTerrain()
    {
        _worldGenerator.GenerateTerrain(atlas.dirt);
        _worldGenerator.GenerateTopBlocks(atlas.grass);
        _worldGenerator.RemoveTopBlocksExcept(atlas.grass);
    }

    public void GenerateTunnels(int bias)
    {
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 4, caveFreq / 8, 3f);
        controller.SetTile(atlas.air);
        controller.SetRenderSettings(0.3f, 0, 0.8f);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: false, heightAffected: false);

        controller.SetRenderSettings(0.7f, 0, 0.5f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true, heightAffected: false);
    }

    public void GenerateDirt(int bias)
    {
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq);
        controller.SetTile(atlas.dirt);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);

        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 6, rarity: 1.5f);

        controller.SetRenderSettings(0.2f, 0, 0.9f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: false);

        controller.SetRenderSettings(0.3f, 0, 0.75f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);

        controller.SetRenderSettings(0.4f, 0, 0.6f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);
    }

    public void GenerateCaves(int bias)
    {
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq);
        controller.SetTile(atlas.stone);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());

        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 4, rarity: 4);
        controller.SetTile(atlas.air);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());

        Tile tile = controller.GetTile();
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 12, rarity: 12);
        controller.SetRenderSettings(tile.renderBorder / 8, tile.maxHeight / 2, tile.minHeight);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());
    }

    public void GenerateOres(int bias)
    {
        Tile[] ores = new Tile[_oresCount] { atlas.coal, atlas.iron, atlas.diamond };
        controller.SetNoiseSettings(caveFreq);

        for (int i = 0; i < _oresCount; i++)
        {
            controller.UpdateRandom();
            controller.SetTile(ores[i]);

            _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);
        }
    }

    public void RenderTiles(Block[,] tiles, GameObject chunk, int bias)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                RenderTile(tiles[j, i], chunk, bias + j, i);
            }
        }
    }

    private void RenderTile(Block tile, GameObject chunk, int x, int y)
    {
        if (tile == null)
            return;

        GameObject tileObject = new(name = tile.name);
        tileObject.transform.parent = chunk.transform;

        //tileObject.AddComponent<BoxCollider2D>();
        //tileObject.tag = "Ground"; // TODO: ENUM CLASS

        tileObject.AddComponent<SpriteRenderer>();
        tileObject.GetComponent<SpriteRenderer>().sprite = tile.sprite;

        tileObject.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}
