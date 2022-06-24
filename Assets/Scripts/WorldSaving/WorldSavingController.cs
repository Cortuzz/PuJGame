using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public static class WorldSavingController
{
    public static void SaveToFile()
    {
        string path = GetPath();
        string data = GenerateJson();

        //saving data
        try
        {
            File.WriteAllText(path, contents: data);
            Debug.Log("World saved. Path: " + path);
        }
        catch (Exception e)
        {
            Debug.Log("World didn't save. Error: " + e.Message);
        }
    }

    public static void LoadFromFile()
    {
        string path = GetPath();

        if (!File.Exists(path))
        {
            Debug.Log("File doesn't exist. Path: " + path);
            return;
        }

        string json = File.ReadAllText(path);

        //reading data
        try
        {
            WorldData worldDataFromJson = JsonUtility.FromJson<WorldData>(json);
            Debug.Log("Data read. Path: " + path);
        }
        catch (Exception e)
        {
            Debug.Log("Daad doesn't read. Error: " + e.Message);
        }
    }

    private static string GenerateJson()
    {
        List<Block> blocks = GetBlocks();

        WorldData worldData = new WorldData
        {
            mobCount = World.mobCount,
            height = World.height,
            width = World.width,
            chunkSize = World.chunkSize,
            chunkCount = World.chunkCount,
            blocks = blocks
        };

        return JsonUtility.ToJson(worldData, true);
    }

    private static List<Block> GetBlocks()
    {
        List<Block> blockList = new List<Block>();
        
        for (int i = 0; i < World.blocks.Count; i++)
        {
            for (int j = 0; j < World.blocks[i].GetLength(0); j++)
            {
                for (int k = 0; k < World.blocks[i].GetLength(1); k++)
                {
                    Block currentBlock =  World.blocks[i][j, k];

                    if (currentBlock != null)
                    {
                        currentBlock.firstIndex = i;
                        currentBlock.secondIndex = j;
                        currentBlock.thirdIndex = k;
                        
                        blockList.Add(currentBlock);
                    }
                }
            }
        }

        for (int i = 0; i < World.backgroundBlocks.Count; i++)
        {
            for (int j = 0; j < World.backgroundBlocks[i].GetLength(0); j++)
            {
                for (int k = 0; k < World.backgroundBlocks[i].GetLength(1); k++)
                {
                    Block currentBlock = World.backgroundBlocks[i][j, k];

                    if (currentBlock != null)
                    {
                        currentBlock.firstIndex = i;
                        currentBlock.secondIndex = j;
                        currentBlock.thirdIndex = k;

                        blockList.Add(currentBlock);
                    }
                }
            }
        }

        return blockList;
    }

    private static string GetPath()
    {
        string path;
        string fileName = "world.json";

#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, saveFileName);
#else
        path = Path.Combine(Application.dataPath, fileName);
#endif

        return path;
    }
}