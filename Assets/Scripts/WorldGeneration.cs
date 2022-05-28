using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public TileAtlas atlas;

    public float caveFreq = 0.08f;
    public float terrainFreq = 0.04f;

    public float heightAdditionRatio = 0.6f;
    public float heightMultiplierRatio = 0.09f;

    private int _x;
    private int _y;

    private readonly float _renderBorder = 0.35f;
    private float _seed;
    private float _heightMultiplier;
    private int _heightAddition;

    private readonly List<Tile> tiles = new List<Tile>();

    // Start is called before the first frame update
    void Start()
    {
        _seed = Random.Range(-1000, 1000);
        _seed = -1035;
        setWorldSize(100, 400);
        GenerateWorld(GenerateNoiseTexture(_x, _y, caveFreq), atlas.stone);
        GenerateWorld(GenerateNoiseTexture(_x, _y, 0.01f), atlas.dirt);
    }

    public void setWorldSize(int x, int y)
    {
        _x = x;
        _y = y;

        _heightAddition = (int)(heightAdditionRatio * _y);
        _heightMultiplier = heightMultiplierRatio * _y;
    }

    private void PlaceTile(int x, int y, Tile tile)
    {
        Tile newTile = tile.prototype();
        Vector2 pos = new Vector2(x + 0.5f, y + 0.5f);
        newTile.pos = pos;

        GameObject tileObject = new(name = newTile.name);
        tileObject.transform.parent = this.transform;
        tileObject.AddComponent<SpriteRenderer>();
        tileObject.GetComponent<SpriteRenderer>().sprite = newTile.sprite;

        tileObject.transform.position = pos;
        tiles.Add(newTile);
    }
        
    public void GenerateWorld(Texture2D texture, Tile tile)
    {
        for (int x = 0; x < texture.width; x++)
        {
            float height = Mathf.PerlinNoise((x + _seed) * terrainFreq, _seed * terrainFreq) * _heightMultiplier + _heightAddition;

            for (int y = 0; y < height; y++)
            {
                if (texture.GetPixel(x, y).r < _renderBorder)
                    continue;

                PlaceTile(x, y, tile);
            }
        }
    }

    public Texture2D GenerateNoiseTexture(int width, int height, float freq)
    {
        Texture2D noiseTexture = new(width, height);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noise = Mathf.PerlinNoise((x + _seed) * freq, (y + _seed) * freq);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }
}
