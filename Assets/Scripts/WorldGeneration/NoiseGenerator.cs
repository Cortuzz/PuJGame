using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    public int bias = 0;
    private int _seed;

    public NoiseGenerator(int seed)
    {
        _seed = seed;
    }

    public Texture2D GetTexture(int width, int height, float horizontalFreq, float verticalFreq, float rarity = 1)
    {
        Texture2D noiseTexture = new(width, height);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noise = GenerateNoise(x, y, horizontalFreq, verticalFreq, rarity);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }

    public Texture2D GetTexture(int width, int height, float freq, float rarity = 1)
    {
        Texture2D noiseTexture = new(width, height);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noise = GenerateNoise(x, y, freq, freq, rarity);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }

    private float GenerateNoise(int x, int y, float hFreq, float vFreq, float rarity)
    {
        float noise = Mathf.PerlinNoise((x + _seed + bias) * hFreq, (y + _seed + bias) * vFreq);
        return Mathf.Pow(noise, rarity);
    }
}
