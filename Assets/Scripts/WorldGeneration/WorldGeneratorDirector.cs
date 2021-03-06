using System.CodeDom.Compiler;
using UnityEngine;

public class WorldGeneratorDirector
{
    public TileAtlas atlas;
    public WorldGeneratorController controller;

    public int seed;
    public float caveFreq = 0.16f;
    public float terrainFreq = 0.04f;

    private WorldGeneration _worldGenerator;

    private const int _oresCount = 3;

    public WorldGeneratorDirector(int seed)
    {
        this.seed = seed;
        controller = new(seed, World.chunkSize, World.height);
    }

    public WorldGeneratorDirector()
    {
        seed = Random.Range(-1000, 1000);
        controller = new(seed, World.chunkSize, World.height);
    }

    public void SetAtlas(TileAtlas atlas)
    {
        this.atlas = atlas;
    }

    public void Pregen(int chunk)
    {
        _worldGenerator = new(seed, terrainFreq);

        controller = new(seed, World.chunkSize, World.height);
        controller.SetBias(chunk * World.chunkSize);
    }

    public void GenerateChunk(bool replacing = false, int chunk = -1)
    {
        var blocks = _worldGenerator.GetBlocks();
        var background = _worldGenerator.GetBackgroundBlocks();

        if (replacing)
        {
            World.ReplaceChunk(chunk, blocks, background);
            return;
        }
        World.AddChunk(blocks, background);
    }

    public void GenerateTrees(int bias)
    {
        _worldGenerator.GenerateTrees(atlas.trunkBottom, atlas.trunkMid, atlas.leaf, bias);
    }

    public void GenerateSand(int bias)
    {
        controller.UpdateRandom();
        controller.SetTile(atlas.sand);
        controller.SetNoiseSettings(caveFreq / 4, caveFreq, rarity: 4f);
        controller.SetTileStats(false);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);
    }

    public void GenerateBackground(int bias)
    {
        controller.UpdateRandom();
        controller.SetTile(atlas.dirt);
        controller.SetNoiseSettings(caveFreq, caveFreq, rarity: 1f);
        controller.SetRenderSettings(0.1f, 0.95f, 0.75f);
        controller.SetTileStats(true);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());

        controller.SetNoiseSettings(caveFreq, caveFreq, rarity: 1f);
        controller.SetTile(atlas.stone);
        controller.SetRenderSettings(0.1f, 0.75f, 0);
        controller.SetTileStats(true);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());
    }

    public void GenerateTerrain()
    {
        _worldGenerator.GenerateTerrain(atlas.dirt);
        _worldGenerator.GenerateTopBlocks(atlas.grass);
        _worldGenerator.RemoveTopBlocksExcept(atlas.grass);
    }

    public void GenerateTunnels(int bias)
    {
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 6, caveFreq / 8, 5f);
        controller.SetTile(atlas.air);
        controller.SetRenderSettings(0.3f, 0, 0.8f);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: false, heightAffected: false);

        controller.SetRenderSettings(0.7f, 0, 0.5f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true, heightAffected: false);

        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 4, caveFreq / 4, rarity: 20f);
        controller.SetTile(atlas.air);
        controller.SetRenderSettings(0.002f, 0.5f, 0);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());
    }

    public void GenerateDirt(int bias)
    {
        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq);
        controller.SetTile(atlas.dirt);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);

        controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 6, rarity: 1.5f);

        controller.SetRenderSettings(0.2f, 0, 0.9f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);

        controller.SetRenderSettings(0.3f, 0, 0.75f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);

        controller.SetRenderSettings(0.4f, 0, 0.6f);
        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);
    }

    public void GenerateCaves(int bias)
    {
        controller.UpdateRandom();
        controller.SetTile(atlas.stone);
        controller.SetNoiseSettings(caveFreq, caveFreq, rarity: 1f);
        controller.SetRenderSettings(0.3f, 0, 0);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());

        /*controller.UpdateRandom();
        controller.SetNoiseSettings(caveFreq / 2, rarity: 4f);
        controller.SetTile(atlas.air);

        _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile());*/
    }

    public void GenerateOres(int bias)
    {
        Tile[] ores = new Tile[_oresCount] { atlas.coal, atlas.iron, atlas.diamond };
        controller.SetNoiseSettings(caveFreq);

        for (int i = 0; i < _oresCount; i++)
        {
            controller.UpdateRandom();
            controller.SetTile(ores[i]);

            _worldGenerator.GenerateTile(controller.GetTexture(), bias, controller.GetTile(), onTiles: true);
        }
    }

    public void DebugHole(ref Block[,] blocks)
    {
        for (int y = 50; y < World.height; y++)
        {
            try { blocks[2, y] = null; }
            catch { }
        }
    }
}
