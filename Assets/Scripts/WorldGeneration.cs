using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGeneration
{
    public float heightAdditionRatio = 0.6f;
    public float heightMultiplierRatio = 0.01f;

    private readonly int _width;
    private readonly int _height;

    private readonly int _seed;
    private readonly float _terrainFreq;
    private readonly float _heightMultiplier;
    private readonly int _heightAddition;
    private readonly int _maxTerrainBlocksCount = 7;

    private readonly Tile[,] _tiles;
    private readonly int[] _heights;

    public WorldGeneration(int w, int h, int seed, float terrainFreq)
    {
        _seed = seed;
        Random.InitState(_seed);

        _terrainFreq = terrainFreq;

        _width = w;
        _height = h;
        _tiles = new Tile[_width, _height];
        _heights = new int[_width];

        _heightAddition = (int)(heightAdditionRatio * _height);
        _heightMultiplier = heightMultiplierRatio * _height;
    }

    private void PlaceTerrainTiles(int x, int y, Tile tile)
    {
        for (int i = 0; i < _maxTerrainBlocksCount; i++)
        {
            PlaceTile(x, y - i, tile);
        }
    }

    public void GenerateTopBlocks(Tile tile)
    {
        for (int i = 0; i < _width; i++)
        {
            PlaceTile(i, _heights[i], tile);
        }
    }

    public void RemoveTopBlocksExcept(Tile tile)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = _height - 1; j >= 0; j--)
            {
                if (_tiles[i, j] != null && _tiles[i, j].name == tile.name)
                    break;

                _tiles[i, j] = null;
            }
        }
    }

    public void GenerateTerrain(Tile tile)
    {
        Queue<int> previousHeights = new();

        for (int i = 0; i < _width; i++)
        {
            for (int j = _height - 1; j >= 0; j--)
            {
                if (_tiles[i, j] != null)
                {
                    previousHeights.Enqueue(j);
                    if (i > 5)
                    {
                        previousHeights.Dequeue();
                    }
                    
                    int average = previousHeights.Sum() / previousHeights.Count + 1;
                    _heights[i] = average;

                    PlaceTerrainTiles(i, average, tile);
                    break;
                }
            }
        }
    }

    private void PlaceTile(int x, int y, Tile tile)
    {
        Tile newTile = tile.Prototype();

        _tiles[x, y] = newTile;
    }

    private bool checkPlacementByNoise(Texture2D texture, Tile tile, int x, int y, int height, bool heightAffected)
    {
        static float sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }

        if (!heightAffected && texture.GetPixel(x, y).r >= tile.renderBorder)
            return true;

        float value = 2 * sigmoid(1 - y / (float)height);
        if (texture.GetPixel(x, y).r >= tile.renderBorder * value)
            return true;

        return false;
    }
        
    public void GenerateTile(Texture2D texture, Tile tile, bool heightAffected = false)
    {
        for (int x = 0; x < texture.width; x++)
        {
            float height = Mathf.PerlinNoise((x + _seed) * _terrainFreq, _seed * _terrainFreq) * _heightMultiplier + _heightAddition;

            for (int y = 0; y < height; y++)
            {
                if (tile.maxHeight > 0 && y / height > tile.maxHeight)
                    break;


                if (!checkPlacementByNoise(texture, tile, x, y, (int)height, heightAffected))
                    continue;

                PlaceTile(x, y, tile);
            }
        }
    }

    public Texture2D GenerateNoiseTexture(float freq)
    {
        Texture2D noiseTexture = new(_width, _height);
        int bias = Random.Range(0, 1000);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noise = Mathf.PerlinNoise((x + _seed + bias) * freq, (y + _seed + bias) * freq);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }

    public Tile[,] GetTiles()
    {
        return _tiles;
    }
}
