using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorController
{
    private int _seed;
    private int _width;
    private int _height;
    private float _verticalFreq;
    private float _horizontalFreq;
    private float _rarity;
    private Tile _tile;

    private NoiseGenerator _noiseGenerator;

    public WorldGeneratorController(int seed, int width, int height)
    {
        _seed = seed;
        _width = width;
        _height = height;

        Random.InitState(_seed);
        _noiseGenerator = new NoiseGenerator(seed);
        UpdateRandom();
    }

    public void SetBias(int bias)
    {
        _noiseGenerator.bias = bias;
    }

    public void UpdateRandom()
    {
        _noiseGenerator.randBias = Random.Range(0, 1000);
    }

    public void SetRenderSettings(float renderBorder, float maxHeight, float minHeight)
    {
        _tile.renderBorder = renderBorder;
        _tile.maxHeight = maxHeight;
        _tile.minHeight = minHeight;
    }

    public void SetTileStats(bool background)
    {
        _tile.isBackground = background;
    }

    public void SetNoiseSettings(float horizontalFreq, float verticalFreq, float rarity = 1)
    {
        _verticalFreq = verticalFreq;
        _horizontalFreq = horizontalFreq;
        _rarity = rarity;
    }

    public void SetNoiseSettings(float freq, float rarity = 1)
    {
        _verticalFreq = freq;
        _horizontalFreq = freq;
        _rarity = rarity;
    }

    public void SetTile(Tile tile)
    {
        _tile = tile.Prototype();
    }

    public Texture2D GetTexture()
    {
        return _noiseGenerator.GetTexture(_width, _height, _horizontalFreq, _verticalFreq, _rarity);
    }

    public Tile GetTile()
    {
        return _tile;
    }
}
