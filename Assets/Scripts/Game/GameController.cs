using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    private readonly Vector2 _tilesOffset = new Vector2(0.5f, 0.5f);

    public GameObject prefab;
    public GameObject map;

    public GameObject audioController;
    public GameObject lightController;
    
    public AudioClip normalMusic;
    public AudioClip bossMusic;

    public GameObject executioner;
    public int count = 0;

    private void Start()
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
        World.craftController = playerController.gameObject.GetComponent<CraftController>();
        map = Instantiate(map, transform.parent);
    }

    public void GenerateChunk(int chunk)
    {
        GameObject chunkObject = new()
        {
            name = $"CHUNK {chunk + 1}",
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
        SpriteRenderer localRenderer = tileObject.GetComponent<SpriteRenderer>();
        localRenderer.sprite = tile.sprite;

        localRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        if (isBackground == 1)
        {
            localRenderer.sortingOrder = -1;
            Color color = localRenderer.color;
            color.r = 0.5f;
            color.g = 0.5f;
            color.b = 0.5f;
            localRenderer.color = color;
        }

        tileObject.transform.position = new Vector2(x + _tilesOffset.x, y + _tilesOffset.y);
        _tileObjects[x, y, isBackground] = tileObject;
    }

    private bool CheckChunkUpdate(int index, float playerPosition)
    {
        return Vector2.Distance(new Vector2(index * World.chunkSize, 0), new Vector2(playerPosition, 0)) <=
               10f * Camera.main.orthographicSize;
    }

    public void UpdateChunks()
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            _chunks[i].SetActive(CheckChunkUpdate(i, playerController.transform.position.x));
        }
    }

    private void Update()
    {
        if (World.isGamePaused) return;
        if (Input.GetKeyDown(KeyCode.M))
        {
            map.GetComponent<MapController>().ChangeVisibility();
        }
    }

    private void FixedUpdate()
    {
        UpdateChunks();
        if (count >= 1) return;
        
        ++count;
        SpawnExecutioner();
        
        GameObject mob = Instantiate(prefab, transform, false);
        var script = mob.GetComponent<Character.Character>();
        script.Spawn(World.width / 2, World.GetHeightAt(World.width / 2));
    }

    public bool TryAddBlock(PlayerController player, Vector2Int roundedPos, bool removingPrimary)
    {
        Item item = player.GetItem();

        if (item == null || item.GetPlacementBlock() == null) return false;
        Block block = item.GetPlacementBlock();
        if (_tileObjects[roundedPos.x, roundedPos.y, 0] != null) return true;
        int b = (removingPrimary) ? 0 : 1;
        if (!removingPrimary && World.GetBlock(roundedPos.x, roundedPos.y, true) != null)
            return true;

        if (!World.CheckNeighbourBlocks(roundedPos.x, roundedPos.y, !removingPrimary))
            return true;

        RenderTile(block, _chunks[roundedPos.x / chunkSize], roundedPos.x, roundedPos.y, b);
        World.SetBlock(roundedPos.x, roundedPos.y, block, !removingPrimary);
        player.inventory.RemoveActiveItem();

        return true;
    }

    public void ObserverUpdate(IObservable observable)
    {
        if (!(observable as PlayerController))
            return;

        var player = observable as PlayerController;
        map.GetComponent<MapController>().UpdateMap(player.GetPosition());

        if (!player.blockAction)
            return;

        bool removingPrimary = player.removingPrimaryBlock;
        Vector2 position = player.GetMousePos();

        Vector2Int roundedPos = new(
            Mathf.RoundToInt(position.x - _tilesOffset.x),
            Mathf.RoundToInt(position.y - _tilesOffset.y)
        );

        GameObject primaryTile = _tileObjects[roundedPos.x, roundedPos.y, 0];
        GameObject secondaryTile = _tileObjects[roundedPos.x, roundedPos.y, 1];

        if (TryAddBlock(player, roundedPos, removingPrimary))
            return;
        
        Item dropItem;
        switch (removingPrimary)
        {
            case true when primaryTile != null:
            {
                dropItem = World.GetBlock(roundedPos.x, roundedPos.y).itemDrop;
                World.SetBlock(roundedPos.x, roundedPos.y, null);
                Destroy(primaryTile);
                break;
            }
            case false when secondaryTile != null:
                dropItem = World.GetBlock(roundedPos.x, roundedPos.y, true).itemDrop;
                World.SetBlock(roundedPos.x, roundedPos.y, null, true);
                Destroy(_tileObjects[roundedPos.x, roundedPos.y, 1]);
                break;
            default:
                return;
        }
        
        if (dropItem == null)
            return;

        var dropItemObject = Instantiate(dropItemPrefab, new Vector2(position.x, position.y), Quaternion.identity);
        var script = dropItemObject.GetComponent<TileDrop>();

        script.item = dropItem; 
        script.Instantiate();
    }

    public void SpawnExecutioner()
    {
        GameObject mob = Instantiate(executioner, transform, false);
        var script = mob.GetComponent<Executioner>();
        var position = playerController.transform.position;
        script.Spawn((int)position.x, (int)position.y + 10);

        var audioSource = audioController.GetComponent<AudioSource>();
        var lighting = lightController.GetComponent<Light2D>();
        var color = new Color(0.4f, 0.1f, 0.1f);
        Camera.main.backgroundColor = color;
        
        lighting.color = color;
        lighting.intensity = 0.6f;
        audioSource.clip = bossMusic;
        audioSource.Play();
    }
}