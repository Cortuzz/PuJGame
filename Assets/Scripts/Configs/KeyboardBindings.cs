using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Configs
{
    public static class KeyboardBindings
    {
        public static Dictionary<string, KeyCode> Config = new()
        {
            { "Left", KeyCode.A },
            { "Right", KeyCode.D },
            { "Jump", KeyCode.Space },
            { "Pause", KeyCode.Escape },
            { "Inventory", KeyCode.I },
            { "Map", KeyCode.M },
        };

        public static bool GetKeyDown(string key)
        {
            return Config.ContainsKey(key) && Input.GetKeyDown(Config[key]);
        }

        public static bool GetKeyUp(string key)
        {
            return Config.ContainsKey(key) && Input.GetKeyUp(Config[key]);
        }

        public static float GetHorizontalKey()
        {
            if (Input.GetKey(Config["Left"])) return -1;
            return Input.GetKey(Config["Right"]) ? 1 : 0;
        }

        public static bool CheckCollisions(string key, int value)
        {
            var isCollided = false;
            
            foreach (var kv in Config)
            {
                if (kv.Key == key || (short)kv.Value != value) continue;
                isCollided = true;
            }

            return isCollided;
        }
    }
}