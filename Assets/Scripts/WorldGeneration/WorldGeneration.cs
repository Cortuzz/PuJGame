using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGeneration
{
    public readonly float heightAdditionRatio = 0.6f;
    public readonly float heightMultiplierRatio = 0.6f;

    public readonly float mountainsSmoothing = 0.25f;
    public readonly float mountainsRarity = 5f;
    public readonly int maxTerrainBlocksCount = 7;

    private readonly int _width;
    private readonly int _height;

    private readonly int _seed;
    private readonly float _terrainFreq;
    private readonly float _heightMultiplier;
    private readonly int _heightAddition;

    private readonly Block[,] _tiles;
    private readonly int[] _heights;

    public WorldGeneration(int w, int h, int seed, float terrainFreq)
    {
        _seed = seed;
        Random.InitState(_seed);

        _terrainFreq = terrainFreq;

        _width = w;
        _height = h;
        _tiles = new Block[_width, _height];
        _heights = new int[_width];

        _heightAddition = (int)(heightAdditionRatio * _height);
        _heightMultiplier = heightMultiplierRatio * _height;
    }

    private void PlaceTerrainTiles(int x, int y, Tile tile)
    {
        for (int i = 0; i < maxTerrainBlocksCount; i++)
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
                if (_tiles[i, j] == null)
                    continue;

                previousHeights.Enqueue(j);
                if (previousHeights.Count > 5)
                    previousHeights.Dequeue();

                int average = previousHeights.Sum() / previousHeights.Count + 1;
                _heights[i] = average;

                PlaceTerrainTiles(i, average, tile);
                break;
            }
        }
    }

    private void PlaceTile(int x, int y, Tile tile)
    {
        if (tile.sprite == null)
        {
            _tiles[x, y] = null;
            return;
        }

        _tiles[x, y] = tile.CreateBlock();
    }

    private bool CheckPlacementByNoise(Texture2D texture, float renderBorder, int x, int y, int bias, int height, bool onTiles)
    {

        if (onTiles && _tiles[x - bias, y] == null)
            return false;

        if (texture.GetPixel(x, y).r >= renderBorder)
            return true;

        return false;
    }
        
    public void GenerateTile(Texture2D texture, int bias, Tile tile, bool heightAffected = true, bool onTiles = false)
    {
        for (int x = 0; x < _width; x++)
        {
            float height = GetTerrainNoise(x + bias);
            float noiseHeight = height;
            if (!heightAffected)
                height = _height;

            for (int y = 0; y < height; y++)
            {
                if (tile.maxHeight > 0 && y / noiseHeight > tile.maxHeight)
                    break;

                if (tile.minHeight > 0 && y / noiseHeight < tile.minHeight)
                    continue;

                if (!CheckPlacementByNoise(texture, tile.renderBorder, x + bias, y, bias, (int)height, onTiles))
                    continue;

                PlaceTile(x, y, tile);
            }
        }
    }

    private float GetTerrainNoise(int x)
    {
        float noise = Mathf.PerlinNoise((x + _seed) * mountainsSmoothing * _terrainFreq, _seed * _terrainFreq);
        return Mathf.Pow(noise, mountainsRarity) * _heightMultiplier + _heightAddition;
    }

    public Block[,] GetBlocks()
    {
        return _tiles;
    }
}
