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

    static public void SetBlock(int chunk, int x, int y, Block block)
    {
        blocks[chunk][x, y] = block;
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
