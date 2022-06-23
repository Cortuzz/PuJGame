using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeClass", menuName = "Recipe Class")]
public class Recipe : ScriptableObject
{
    public Item[] _components;
    public int[] _componentsCount;
    public Item _item;
    public int _itemCount;

    public bool CanCraft(Inventory inventory)
    {
        return !_components.Where((t, i) => !inventory.IsContains(t, _componentsCount[i])).Any();
    }

    public void Craft(Inventory inventory)
    {
        for (var i = 0; i < _components.Length; i++)
        {
            inventory.Remove(_components[i], _componentsCount[i]);
        }

        for (var i = 0; i < _itemCount; i++)
        {
            inventory.TryAdd(_item);
        }
    }
}
