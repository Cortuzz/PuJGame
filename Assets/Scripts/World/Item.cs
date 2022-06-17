using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item Class")]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    Tile block = null;
}
