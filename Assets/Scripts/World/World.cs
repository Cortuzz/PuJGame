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

    static public void SetWorldInfo(int height_, int chunkSize_, int chunkCount_)
    {
        height = height_;
        chunkSize = chunkSize_;
        chunkCount = chunkCount_;
        width = chunkSize * chunkCount;
        blocks = new List<Block[,]>();
    }

    static public void SetBlocks(List<Block[,]> blocks_)
    {
        blocks = blocks_;
    }

    static public void SetBlock(int chunk, int x, int y, Block block)
    {
        blocks[chunk][x, y] = block;
    }

    static public void AddChunk(Block[,] chunk)
    {
        blocks.Add(chunk);
    }
}
