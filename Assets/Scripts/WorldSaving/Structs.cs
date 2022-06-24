using System.Collections.Generic;

[System.Serializable]
public struct WorldData
{
    public int mobCount;
    public int height;
    public int width;
    public int chunkSize;
    public int chunkCount;
    public List<Block> blocks;
}

[System.Serializable]
public struct BlockData
{
    public string name;
    public bool isBackground;
    public int x;
    public int y;
}