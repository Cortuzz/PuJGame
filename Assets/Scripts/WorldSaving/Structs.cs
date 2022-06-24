using System.Collections.Generic;

[System.Serializable]
public struct WorldData
{
    public int mobCount;
    public int height;
    public int width;
    public int chunkSize;
    public int chunkCount;
    public List<Block[,]> blocks;
    public List<Block[,]> backgroundBlocks;
}