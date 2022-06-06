using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TileAtlas tileAtlas;
    public PlayerController playerController;
    public WorldGeneratorDirector worldGeneratorDirector;

    public int height;
    public int chunkSize;
    public int chunksCount;

    private readonly List<GameObject> _chunks = new();

    void Start()
    {
        World.SetWorldInfo(height, chunkSize, chunksCount);
        worldGeneratorDirector = new();
        worldGeneratorDirector.SetAtlas(tileAtlas);
        worldGeneratorDirector.GenerateChunks();

        playerController.Spawn(World.width / 2, World.height - 5);

        for (int chunk = 0; chunk < chunksCount; chunk++)
        {
            GenerateChunk(chunk);
        }
    }

    public void GenerateChunk(int chunk)
    {
        GameObject chunkObject = new()
        {
            name = "CHUNK " + (chunk + 1),
            isStatic = true
        };
        chunkObject.transform.parent = transform;

        _chunks.Add(chunkObject);
        RenderTiles(World.blocks[chunk], chunkObject, chunk * World.chunkSize);
        RenderTiles(World.backgroundBlocks[chunk], chunkObject, chunk * World.chunkSize);
    }

    public void RenderTiles(Block[,] tiles, GameObject chunk, int bias)
    {

        for (int i = 0; i < World.height; i++)
        {
            for (int j = 0; j < World.chunkSize; j++)
            {
                RenderTile(tiles[j, i], chunk, bias + j, i);
            }
        }
    }

    private void RenderTile(Block tile, GameObject chunk, int x, int y)
    {
        if (tile == null)
            return;

        GameObject tileObject = new(name = tile.name);
        tileObject.transform.parent = chunk.transform;
        tileObject.isStatic = true;

        tileObject.AddComponent<SpriteRenderer>();
        SpriteRenderer renderer = tileObject.GetComponent<SpriteRenderer>();
        renderer.sprite = tile.sprite;

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        if (tile.isBackground)
        {
            renderer.sortingOrder = -1;
            Color color = renderer.color;
            color.a = 0.5f;
            renderer.color = color;
        }

        //tileObject.AddComponent<MeshRenderer>();
        //tileObject.GetComponent<MeshRenderer>().receiveShadows = true;


        tileObject.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }

    private bool CheckChunkUpdate(int index, float playerPosition)
    {
        return Vector2.Distance(new Vector2(index * World.chunkSize, 0), new Vector2(playerPosition, 0)) <= 10f * Camera.main.orthographicSize;
    }

    public void UpdateChunks()
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            _chunks[i].SetActive(CheckChunkUpdate(i, playerController.transform.position.x));
        }
    }

    void FixedUpdate()
    {
        UpdateChunks();
    }
}
