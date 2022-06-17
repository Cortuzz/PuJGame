using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IObserver
{
    public TileAtlas tileAtlas;
    public GameObject dropItemPrefab;
    
    public PlayerController playerController;
    public WorldGeneratorDirector worldGeneratorDirector;

    public int height;
    public int chunkSize;
    public int chunksCount;

    private readonly List<GameObject> _chunks = new();
    private GameObject[,,] _tileObjects;
    private Vector2 _tilesOffset = new Vector2(0.5f, 0.5f);

    public GameObject prefab;
    public int count = 0;

    void Start()
    {
        World.SetWorldInfo(height, chunkSize, chunksCount);
        _tileObjects = new GameObject[chunksCount * chunkSize, height, 2];  

        worldGeneratorDirector = new();
        worldGeneratorDirector.SetAtlas(tileAtlas);
        worldGeneratorDirector.GenerateChunks();

        for (int chunk = 0; chunk < chunksCount; chunk++)
        {
            GenerateChunk(chunk);
        }

        playerController.Attach(this);
        playerController.Spawn(World.width / 2, World.GetHeightAt(World.width / 2));
        World.SetPlayer(playerController);
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
        RenderTiles(World.backgroundBlocks[chunk], chunkObject, chunk * World.chunkSize, 1);
    }

    public void RenderTiles(Block[,] tiles, GameObject chunk, int bias, int isBackground = 0)
    {

        for (int i = 0; i < World.height; i++)
        {
            for (int j = 0; j < World.chunkSize; j++)
            {
                RenderTile(tiles[j, i], chunk, bias + j, i, isBackground);
            }
        }
    }

    private void RenderTile(Block tile, GameObject chunk, int x, int y, int isBackground)
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


        tileObject.transform.position = new Vector2(x + _tilesOffset.x, y + _tilesOffset.y);
        _tileObjects[x, y, isBackground] = tileObject;
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
        if (count < 1 && Random.Range(0, 100) == 1)
        {
            ++count;
            GameObject mob = Instantiate(prefab, transform, false);
            var script = mob.GetComponent<Character>();
            script.Spawn(World.width / 2, World.GetHeightAt(World.width / 2));
        }
    }

    public void ObserverUpdate(IObservable observable)
    {
        if ((observable as PlayerController).removingBlock)
        {
            Vector2 position = (observable as PlayerController).GetMousePos();

            Vector2Int roundedPos = new(
                Mathf.RoundToInt(position.x - _tilesOffset.x),
                Mathf.RoundToInt(position.y - _tilesOffset.y)
            );

            GameObject primaryTile = _tileObjects[roundedPos.x, roundedPos.y, 0];
            if (primaryTile != null)
            {
                var dropItem = World.GetBlock(roundedPos.x, roundedPos.y).itemDrop;
                var dropItemObject = Instantiate(dropItemPrefab, new Vector2(position.x, position.y), Quaternion.identity);
                var script = dropItemObject.GetComponent<TileDrop>();
                script.item = dropItem;

                World.SetBlock(roundedPos.x, roundedPos.y, null);
                Destroy(primaryTile);

                script.Instantiate();
                return;
            }
            World.SetBlock(roundedPos.x, roundedPos.y, null, true);
            Destroy(_tileObjects[roundedPos.x, roundedPos.y, 1]);
        }
    }
}
