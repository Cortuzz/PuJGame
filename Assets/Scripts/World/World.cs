using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World
{
    static public int height;
    static public int width;
    static public int chunkSize;
    static public int chunkCount;
    static public List<Block[,]> blocks;
    static public List<Block[,]> backgroundBlocks;
    static public Character player;

    static public void SetWorldInfo(int height_, int chunkSize_, int chunkCount_)
    {
        height = height_;
        chunkSize = chunkSize_;
        chunkCount = chunkCount_;
        width = chunkSize * chunkCount;
        blocks = new List<Block[,]>();
        backgroundBlocks = new List<Block[,]>();
    }

    static public void SetPlayer(Character player_)
    {
        player = player_;
    }

    static public void SetBlocks(List<Block[,]> blocks_)
    {
        blocks = blocks_;
    }

    static public Block GetBlock(int chunk, int x, int y, bool isBackground = false)
    {
        if (isBackground)
            return backgroundBlocks[chunk][x, y];
        return blocks[chunk][x, y];
    }

    static public Block GetBlock(int x, int y, bool isBackground = false)
    {
        return GetBlock(x / chunkSize, x % chunkSize, y, isBackground);
    }

    static public void SetBlock(int chunk, int x, int y, Block block, bool isBackground = false)
    {
        if (isBackground)
        {
            backgroundBlocks[chunk][x, y] = block;
            return;
        }
        blocks[chunk][x, y] = block;
    }

    static public void SetBlock(int x, int y, Block block, bool isBackground = false)
    {
        SetBlock(x / chunkSize, x % chunkSize, y, block, isBackground);
    }

    static public bool CheckNeighbourBlocks(int x, int y, bool isBackground)
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

    static public void AddChunk(Block[,] chunk, Block[,] bgChunk)
    {
        blocks.Add(chunk);
        backgroundBlocks.Add(bgChunk);
    }

    static public int GetHeightAt(int x)
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
