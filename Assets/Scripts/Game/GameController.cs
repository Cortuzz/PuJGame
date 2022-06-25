using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour, IObserver
{
    public GameObject dropItemPrefab;

    public PlayerController playerController;

    private readonly List<GameObject> _chunks = new();
    private GameObject[,,] _tileObjects;
    private readonly Vector2 _tilesOffset = new Vector2(0.5f, 0.5f);

    public GameObject prefab;
    public GameObject map;

    public GameObject audioController;
    public GameObject lightController;
    public GameObject progressBarPrefab;
    public GameObject progressBar;
    
    public AudioClip normalMusic;
    public AudioClip bossMusic;


    public bool bossSpawned;
    public GameObject executioner;
    public int count = 0;
    public Slider slider;
    public Executioner boss;

    private void Start()
    {
        World.audioController = audioController;
        _tileObjects = new GameObject[World.chunkCount * World.chunkSize, World.height, 2];

        for (int chunk = 0; chunk < World.chunkCount; chunk++)
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
        if (KeyboardBindings.GetKeyDown("Map"))
        {
            map.GetComponent<MapController>().ChangeVisibility();
        }
    }

    private void CheckBossDeath()
    {
        if (!(World.destroyBoss))
            return;

        try
        {
            Destroy(boss.gameObject);
            Destroy(progressBar);
            World.destroyBoss = false;
            bossSpawned = false;
            World.boss = false;

            var audioSource = audioController.GetComponent<AudioSource>();
            var lighting = lightController.GetComponent<Light2D>();
            var color = new Color(0.1921569f, 0.3019608f, 0.4745098f);
            Camera.main.backgroundColor = color;

            lighting.color = new Color(1f, 1f, 1f);
            lighting.intensity = 1f;
            audioSource.clip = normalMusic;
            audioSource.Play();
        }
        catch (Exception e)
        {
            
        }
       
    }

    private void FixedUpdate()
    {
        UpdateChunks();
        SpawnMobs();

        if (!bossSpawned)
            return;

        slider.value = (float)boss.health / boss.maxHealth;
        CheckBossDeath();
    }

    public void CheckBossSpawn(Vector2Int position)
    {
        try
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();

            var headsCount = 0;
            var sandCount = 0;

            for (int i = position.x - 1; i <= position.x + 1; i++)
            {
                if (World.GetBlock(i, position.y).name == "Head")
                {
                    headsCount++;
                    toRemove.Add(new Vector2Int(i, position.y));
                }
            }

            if (headsCount < 3)
                return;

            for (int i = position.x - 1; i <= position.x + 1; i++)
            {
                if (World.GetBlock(i, position.y - 1).name == "Sand")
                {
                    sandCount++;
                    toRemove.Add(new Vector2Int(i, position.y - 1));
                }
            }

            if (sandCount < 3)
                return;

            if (World.GetBlock(position.x - 1, position.y - 2) != null ||
                World.GetBlock(position.x + 1, position.y - 2) != null ||
                World.GetBlock(position.x, position.y - 2).name != "Sand")
                return;
            
            World.SetBlock(position.x, position.y - 2, null);
            Destroy(_tileObjects[position.x, position.y - 2, 0]);

            foreach (var vector in toRemove)
            {
                World.SetBlock(vector.x, vector.y, null);
                Destroy(_tileObjects[vector.x, vector.y, 0]);
            }

            SpawnExecutioner();
        }
        catch (Exception e)
        {
            
        }
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

        RenderTile(block, _chunks[roundedPos.x / World.chunkSize], roundedPos.x, roundedPos.y, b);
        World.SetBlock(roundedPos.x, roundedPos.y, block, !removingPrimary);
        player.inventory.RemoveActiveItem();
        
        if (block.name == "Head")
            CheckBossSpawn(roundedPos);

        return true;
    }

    public void SpawnMobs()
    {
        if (World.mobCount >= 3 || Random.Range(0, 2500) > 0 || bossSpawned)
            return;

        var position = playerController.GetPosition();
        var chunk = (int)position.x / World.chunkSize;
        var spawnPositions = new List<Vector2Int>();

        for (var i = 2; i < World.chunkSize - 2; i++)
        {
            for (var j = (int)position.y - 60; j < position.y + 20; j++)
            {
                try
                {
                    var f1 = World.GetBlock(chunk, i, j) != null || World.GetBlock(chunk, i - 1, j) != null ||
                             World.GetBlock(chunk, i + 1, j) != null;
                    var f2 = World.GetBlock(chunk, i, j - 1) == null || World.GetBlock(chunk, i - 1, j - 1) == null ||
                             World.GetBlock(chunk, i + 1, j - 1) == null;

                    if (f1 || f2)
                        continue;

                    spawnPositions.Add(new Vector2Int(i, j));
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        if (spawnPositions.Count == 0)
            return;

        var randIndex = Random.Range(0, spawnPositions.Count);
        var pos = spawnPositions[randIndex];
        
        GameObject mob = Instantiate(prefab, transform, false);
        var script = mob.GetComponent<Character.Character>();
        script.Spawn(pos.x + chunk * World.chunkSize, pos.y);
        World.mobCount++;
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
        World.destroyBoss = false;
        progressBar = Instantiate(progressBarPrefab, transform.parent);
        progressBar.transform.GetChild(0).localPosition = new Vector3(0, -210, 0);
        slider = progressBar.transform.GetChild(0).gameObject.GetComponent<Slider>();
        
        GameObject mob = Instantiate(executioner, transform, false);
        var script = mob.GetComponent<Executioner>();
        boss = script;
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
        bossSpawned = true;
        World.boss = true;
    }
}