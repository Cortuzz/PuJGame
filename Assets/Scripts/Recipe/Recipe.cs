using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeClass", menuName = "Recipe Class")]
public class Recipe : ScriptableObject
{
    [SerializeField] private Item[] _components;
    [SerializeField] private int[] _componentsCount;
    [SerializeField] private Item _item;
    [SerializeField] private int _itemCount;

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
    }
}
