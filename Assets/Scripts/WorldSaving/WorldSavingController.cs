using System;
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
        WorldData worldData = new WorldData
        {
            mobCount = World.mobCount,
            height = World.height,
            width = World.width,
            chunkSize = World.chunkSize,
            chunkCount = World.chunkCount,
            blocks = World.blocks,
            backgroundBlocks = World.backgroundBlocks
        };

        return JsonUtility.ToJson(worldData, true);
    }

    private static string GetPath()
    {
        string path;
        string fileName = "data.json";

#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, saveFileName);
#else
        path = Path.Combine(Application.dataPath, fileName);
#endif

        return path;
    }
    
    
}