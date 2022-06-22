using System.Collections.Generic;
using UnityEngine;

public static class World
{
    public static int height;
    public static int width;
    public static int chunkSize;
    public static int chunkCount;
    public static List<Block[,]> blocks;
    public static List<Block[,]> backgroundBlocks;
    public static Character.Character player;
    public static bool isGamePaused;

    private static bool _isMapOpened;

    public static void SetWorldInfo(int height_, int chunkSize_, int chunkCount_)
    {
        height = height_;
        chunkSize = chunkSize_;
        chunkCount = chunkCount_;
        width = chunkSize * chunkCount;
        blocks = new List<Block[,]>();
        backgroundBlocks = new List<Block[,]>();
    }

    public static void SetMapState(bool state)
    {
        _isMapOpened = state;
    }

    public static bool CanPlay()
    {
        return !_isMapOpened && !isGamePaused;
    }

    public static void SetPlayer(Character.Character player_)
    {
        player = player_;
    }

    public static void SetBlocks(List<Block[,]> blocks_)
    {
        blocks = blocks_;
    }

    public static Block GetBlock(int chunk, int x, int y, bool isBackground = false)
    {
        if (isBackground)
            return backgroundBlocks[chunk][x, y];
        return blocks[chunk][x, y];
    }

    public static Block GetBlock(int x, int y, bool isBackground = false)
    {
        return GetBlock(x / chunkSize, x % chunkSize, y, isBackground);
    }

    public static void SetBlock(int chunk, int x, int y, Block block, bool isBackground = false)
    {
        if (isBackground)
        {
            backgroundBlocks[chunk][x, y] = block;
            return;
        }
        blocks[chunk][x, y] = block;
    }

    public static void SetBlock(int x, int y, Block block, bool isBackground = false)
    {
        SetBlock(x / chunkSize, x % chunkSize, y, block, isBackground);
    }

    public static bool CheckNeighbourBlocks(int x, int y, bool isBackground)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;

                if (GetBlock(x + i, y + j, isBackground) != null)
                    return true;
            }
        }

        return false;
    }

    public static void AddChunk(Block[,] chunk, Block[,] bgChunk)
    {
        blocks.Add(chunk);
        backgroundBlocks.Add(bgChunk);
    }

    public static int GetHeightAt(int x)
    {
        int chunkNumber = x / chunkSize;
        x %= chunkSize;

        return GetHeightAt(chunkNumber, x);
    }

    static public int GetHeightAt(int chunk, int x)
    {
        Block[,] currentChunk = blocks[chunk];
        for (int y = height - 1; y >= 0; --y)
        {
            if (currentChunk[x, y] != null)
                return y;
        }
        return -1;
    }
}
