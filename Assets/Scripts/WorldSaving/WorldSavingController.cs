using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WorldSaving
{
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

        //обновляет некоторые поля в статичном классе World исходя из последнего сохранения
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
            WorldData worldDataFromJson = JsonUtility.FromJson<WorldData>(json);
            Debug.Log("Data read. Path: " + path);

            World.width = worldDataFromJson.width;
            World.height = worldDataFromJson.height;
            World.chunkSize = worldDataFromJson.chunkSize;
            World.chunkCount = worldDataFromJson.chunkCount;
            World.mobCount = worldDataFromJson.mobCount;

            var jsonBlocks = worldDataFromJson.blocks;

            var worldBlocks = new List<Block[,]>();
            var worldBackgroundBlocks = new List<Block[,]>();


            for (var i = 0; i < World.chunkCount; i++)
            {
                worldBlocks.Add(new Block[World.width, World.height]);
                worldBackgroundBlocks.Add(new Block[World.width, World.height]);
            }

            foreach (var currentBlock in jsonBlocks)
            {
                Debug.Log(currentBlock.firstIndex);
                Debug.Log(currentBlock.secondIndex);
                Debug.Log(currentBlock.thirdIndex);

                if (!currentBlock.isBackground)
                {
                    worldBlocks[currentBlock.firstIndex][currentBlock.secondIndex, currentBlock.thirdIndex] =
                        currentBlock;
                }
                else
                {
                    worldBackgroundBlocks[currentBlock.firstIndex][currentBlock.secondIndex, currentBlock.thirdIndex] =
                        currentBlock;
                }
            }

            World.blocks = worldBlocks;
            World.backgroundBlocks = worldBackgroundBlocks;
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
            Debug.Log(World.blocks.Count);
            Debug.Log(World.blocks[0].GetLength(0));
            Debug.Log(World.blocks[0].GetLength(1));
            for (int i = 0; i < World.blocks.Count; i++)
            {
                for (int j = 0; j < World.blocks[i].GetLength(0); j++)
                {
                    for (int k = 0; k < World.blocks[i].GetLength(1); k++)
                    {
                        Block currentBlock = World.blocks[i][j, k];

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
}