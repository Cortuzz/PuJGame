using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public Sprite tileSprite;

    private float _seed;
    private float _renderBorder = 0.45f;
    public float noiseFreq = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        _seed = Random.Range(-1000, 1000);
        GenerateWorld(GenerateNoiseTexture(500, 700));
    }

    public void GenerateWorld(Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).r > _renderBorder)
                    continue;

                GameObject tile = new GameObject(name = "tile");
                tile.AddComponent<SpriteRenderer>();
                tile.GetComponent<SpriteRenderer>().sprite = tileSprite;
                tile.transform.position = new Vector2(x, y);
            }
        }
    }

    public Texture2D GenerateNoiseTexture(int width, int height)
    {
        Texture2D noiseTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise((x + _seed) * noiseFreq, (y + _seed) * noiseFreq);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }
}
