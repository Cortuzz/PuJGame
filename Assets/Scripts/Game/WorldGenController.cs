using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGenController : MonoBehaviour
{
    public GameObject chunkPrefab;
    public TileAtlas tileAtlas;
    public WorldGeneratorDirector worldGeneratorDirector;
    public Texture2D chunksPreview;
    public GameObject chunksObject;

    public int height;
    public int chunkSize;
    public int chunksCount;
    public int index;
    public int genIndex;
    public int renderTimer;
    public int initialRenderTimer = 300;

    void Start()
    {
        World.SetWorldInfo(height, chunkSize, chunksCount);
        chunksPreview = new Texture2D(World.width, World.height);
        var chunk = Instantiate(chunkPrefab, transform);
        chunk.AddComponent<RawImage>().texture = chunksPreview;
        chunksObject = chunk;
        
        SetupTexture();

        worldGeneratorDirector = new();
        worldGeneratorDirector.SetAtlas(tileAtlas);
        worldGeneratorDirector.Pregen(0);
        chunksObject.transform.GetChild(0).GetComponent<TMP_Text>().text = $"Generating Caves\nChunk: 1 / {World.chunkCount}";
    }
    void SetupTexture()
    {
        for (int chunk = 0; chunk < World.chunkCount; chunk++)
            for (int i = 0; i < World.chunkSize; i++)
                for (int j = 0; j < World.height; j++)
                    chunksPreview.SetPixel(i + chunk * World.chunkSize, j, new(0, 0, 0, 1));

        chunksPreview.Apply();
    }

    private void UpdateBlock(int chunk, int x, int y, Block block, bool updated = false)
    {
        if (!updated && block == null)
        {
            chunksPreview.SetPixel(x + chunk * World.chunkSize, y, new(1, 1, 1, 0));
            return;
        }

        if (block == null)
            return;

        var color = block.color;
        if (!updated)
        {
            color.r /= 2;
            color.b /= 2;
            color.g /= 2;
        }

        chunksPreview.SetPixel(x + chunk * World.chunkSize, y, color);
    }

    public void UpdateMap(int chunk)
    {
        for (int i = 0; i < World.chunkSize	; i++)
        {
            for (int j = 0; j < height; j++)
            {
                UpdateBlock(chunk, i, j, World.GetBlock(chunk, i, j, true));
                UpdateBlock(chunk, i, j, World.GetBlock(chunk, i, j), true);
            }
        }
        
        chunksPreview.Apply();
    }
    
    private void GenerationQueue(int bias)
    {
        var text = "";
        switch (genIndex)
        {
            case 0:
                text = "Generating Dirt";
                worldGeneratorDirector.GenerateCaves(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk();
                UpdateMap(index);
                break;
            case 1:
                text = "Generating Ores";
                worldGeneratorDirector.GenerateDirt(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 2:
                text = "Generating Terrain";
                worldGeneratorDirector.GenerateOres(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 3:
                text = "Generating Large Caves";
                worldGeneratorDirector.GenerateTerrain();
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 4:
                text = "Generating Background";
                worldGeneratorDirector.GenerateTunnels(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 5:
                text = "Generating Sand";
                worldGeneratorDirector.GenerateBackground(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 6:
                text = "Generating Trees";
                worldGeneratorDirector.GenerateSand(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            case 7:
                text = "Finishing Up";
                worldGeneratorDirector.GenerateTrees(bias * World.chunkSize);
                worldGeneratorDirector.GenerateChunk(true, bias);
                UpdateMap(index);
                break;
            default:
                UpdateMap(index);
                index++;
                text = "Generating Caves";
                worldGeneratorDirector.GenerateChunk(true, bias);
                worldGeneratorDirector.Pregen(index);
                genIndex = -1;
                if (index == World.chunkCount)
                {
                    chunksObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Finishing Up";
                    return;
                }

                
                break;
        }

        chunksObject.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{text}\nChunk: {index + 1} / {World.chunkCount}";
        genIndex++;
    }

    private void FixedUpdate()
    {
        renderTimer = Mathf.Max(renderTimer - 1, 0);
        if (renderTimer > 0)
            return;

        renderTimer = initialRenderTimer;
        
        if (index >= World.chunkCount)
        {
            SceneManager.LoadScene("SampleScene");
            Destroy(gameObject);
            return;
        }
        
        GenerationQueue(index);
    }
}
