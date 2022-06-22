using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGeneration
{
    public readonly float heightAdditionRatio = 0.6f;
    public readonly float heightMultiplierRatio = 0.6f;

    public readonly float mountainsSmoothing = 0.15f;
    public readonly float mountainsRarity = 5f;
    public readonly int maxTerrainBlocksCount = 7;

    public readonly int minTreeHeight = 4;
    private readonly int maxTreeHeight = 10;

    private readonly int _seed;
    private readonly float _terrainFreq;
    private readonly float _heightMultiplier;
    private readonly int _heightAddition;

    private readonly Block[,] _tiles;
    private readonly Block[,] _backgroundTiles;
    private readonly int[] _heights;

    public WorldGeneration(int seed, float terrainFreq)
    {
        _seed = seed;
        Random.InitState(_seed);

        _terrainFreq = terrainFreq;

        _tiles = new Block[World.chunkSize, World.height];
        _backgroundTiles = new Block[World.chunkSize, World.height];
        _heights = new int[World.chunkSize];

        _heightAddition = (int)(heightAdditionRatio * World.height);
        _heightMultiplier = heightMultiplierRatio * World.height;
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
        for (int i = 0; i < World.chunkSize; i++)
        {
            PlaceTile(i, _heights[i], tile);
        }
    }

    public void RemoveTopBlocksExcept(Tile tile)
    {
        for (int i = 0; i < World.chunkSize; i++)
        {
            for (int j = World.height - 1; j >= 0; j--)
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

        for (int i = 0; i < World.chunkSize; i++)
        {
            for (int j = World.height - 1; j >= 0; j--)
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
            GetBlocksArray(tile.isBackground)[x, y] = null;
            return;
        }

        GetBlocksArray(tile.isBackground)[x, y] = tile.CreateBlock();
    }

    private bool CheckPlacementByNoise(Texture2D texture, float renderBorder, int x, int y, int bias, int height, bool onTiles)
    {
        if (onTiles && _tiles[x - bias, y] == null)
            return false;

        if (texture.GetPixel(x, y).r >= renderBorder)
            return true;

        return false;
    }

    private void GenerateTree(Tile bottom, Tile mid, Tile leaf, int x)
    {
        var treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        var height = _heights[x] + 1;
        
        PlaceTile(x, height, bottom);
        for (var i = 1; i < treeHeight; i++)
            PlaceTile(x, height + i, mid);

        for (var i = x - 5; i < x + 5; i++)
        {
            for (var j = height + treeHeight - 3; j < height + treeHeight + 5; j++)
            {
                if (Mathf.Pow(x - i, 2) + Mathf.Pow(height + treeHeight - j, 2) > 9)
                    continue;
                if (i < 0 || i >= World.chunkSize)
                    continue;
                
                PlaceTile(i, j, leaf);
            }
        }
    }

    public void GenerateTrees(Tile bottom, Tile mid, Tile leaf)
    {
        UpdateHeights();
        for (int x = 0; x < World.chunkSize; x++)
        {
            if (Random.Range(0, 10) != 0)
                continue;
            
            GenerateTree(bottom, mid, leaf, x);
        }
    }
        
    public void GenerateTile(Texture2D texture, int bias, Tile tile, bool heightAffected = true, bool onTiles = false)
    {
        for (int x = 0; x < World.chunkSize; x++)
        {
            float height = GetTerrainNoise(x + bias);
            float noiseHeight = height;
            if (!heightAffected)
                height = World.height;

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

    public void UpdateHeights()
    {
        for (int i = 0; i < World.chunkSize; i++)
        {
            for (int j = World.height - 1; j >= 0; j--)
            {
                if (_tiles[i, j] != null)
                {
                    _heights[i] = j;
                    break;
                }
            }
        }
    }

    private Block[,] GetBlocksArray(bool bg)
    {
        if (bg)
            return _backgroundTiles;
        return _tiles;
    }

    public Block[,] GetBlocks()
    {
        return _tiles;
    }

    public Block[,] GetBackgroundBlocks()
    {
        return _backgroundTiles;
    }
}
