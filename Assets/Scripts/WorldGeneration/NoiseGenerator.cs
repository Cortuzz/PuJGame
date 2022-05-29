using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    public int _seed;

    public NoiseGenerator(int seed)
    {
        _seed = seed;
        Random.InitState(_seed);
    }

    public Texture2D GetTexture(int width, int height, float freq, bool multiNoise = false)
    {
        Texture2D noiseTexture = new(width, height);
        int bias = Random.Range(0, 1000);

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float noise = GenerateNoise(x, y, bias, freq, multiNoise);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTexture.Apply();
        return noiseTexture;
    }

    private float GenerateNoise(int x, int y, int bias, float freq, bool multiNoise)
    {
        if (!multiNoise)
            return Mathf.PerlinNoise((x + _seed + bias) * freq, (y + _seed + bias) * freq);

        float noise1 = Mathf.PerlinNoise((x + _seed + bias) * freq, (y + _seed + bias) * freq);
        float noise2 = Mathf.PerlinNoise((x + _seed + bias) * 2 * freq, (y + _seed + bias) * 2 * freq);
        float noise3 = Mathf.PerlinNoise((x + _seed + bias) * 4 * freq, (y + _seed + bias) * 2 * freq);

        return 1f * noise1 + 0.5f * noise2 + 0.2f * noise3;
    }
}
