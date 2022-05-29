using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorDirector : MonoBehaviour
{
    public TileAtlas atlas;

    public int height = 400;
    public int width;
    public int chunkSize = 64;
    public int chunksCount = 2;

    public int seed;
    public float caveFreq = 0.08f;
    public float terrainFreq = 0.04f;

    private WorldGeneration _worldGenerator;
    private NoiseGenerator _noiseGenerator;

    private Texture2D _caveTexture;
    private Texture2D _bigCaveTexture;
    private Texture2D _dirtTexture;
    private Texture2D[] _oresTextures;
    private const int _oresCount = 3;

    void Start()
    {
        seed = Random.Range(-1000, 1000);
        width = chunksCount * chunkSize;
        _noiseGenerator = new(seed);

        GenerateChunks();
    }

    public void GenerateChunks()
    {
        _caveTexture = _noiseGenerator.GetTexture(width, height, caveFreq);
        _dirtTexture = _noiseGenerator.GetTexture(width, height, caveFreq);
        _bigCaveTexture = _noiseGenerator.GetTexture(width, height, caveFreq / 4);
        _oresTextures = new Texture2D[_oresCount];

        for (int i = 0; i < _oresCount; i++)
            _oresTextures[i] = _noiseGenerator.GetTexture(width, height, caveFreq);

        for (int chunk = 0; chunk < chunksCount; chunk++)
        {
            GameObject chunkObject = new();
            chunkObject.name = "CHUNK " + (chunk + 1);
            chunkObject.transform.parent = this.transform;

            _worldGenerator = new(chunkSize, height, seed, terrainFreq);
            GenerateChunk(chunk * chunkSize);
            RenderTiles(_worldGenerator.GetTiles(), chunkObject, chunk * chunkSize);
        }
       
    }

    public void GenerateChunk(int bias)
    {
        GenerateCaves(bias);
        _worldGenerator.GenerateTile(_dirtTexture, bias, atlas.dirt, onTiles: true);
        GenerateOres(bias);

        _worldGenerator.GenerateTerrain(atlas.dirt);
        _worldGenerator.GenerateTopBlocks(atlas.grass);
        _worldGenerator.RemoveTopBlocksExcept(atlas.grass);
    }

    public void GenerateCaves(int bias)
    {
        _worldGenerator.GenerateTile(_caveTexture, bias, atlas.stone);
        _worldGenerator.GenerateTile(_bigCaveTexture, bias, atlas.air);
    }

    public void GenerateOres(int bias)
    {
        Tile[] ores = new Tile[_oresCount] { atlas.coal, atlas.iron, atlas.diamond };

        for (int i = 0; i < _oresCount; i++)
        {
            _worldGenerator.GenerateTile(_oresTextures[i], bias, ores[i], onTiles: true);
        }
    }

    public void RenderTiles(Tile[,] tiles, GameObject chunk, int bias)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                RenderTile(tiles[j, i], chunk, bias + j, i);
            }
        }
    }

    private void RenderTile(Tile tile, GameObject chunk, int x, int y)
    {
        if (tile == null)
            return;

        GameObject tileObject = new(name = tile.name);
        tileObject.transform.parent = chunk.transform;

        tileObject.AddComponent<BoxCollider2D>();
        //tileObject.tag = "Ground"; // TODO: ENUM CLASS

        tileObject.AddComponent<SpriteRenderer>();
        tileObject.GetComponent<SpriteRenderer>().sprite = tile.sprite;

        tileObject.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}
