using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftController : MonoBehaviour
{
    [SerializeField] private GameObject _craftUI;
    [SerializeField] private GameObject _recipeSlotPrefab;
    [SerializeField] private Recipe[] _recipes;
    private List<GameObject> _recipesUI;

    public Vector2 multiplierUI;
    public Vector2 offsetUI;

    public void Craft(int index)
    {
        var recipe = _recipes[index];
        recipe.Craft(World.player.GetInventory());
    }
    private void Start()
    {
        _recipesUI = new List<GameObject>();
        for (var i = 0; i < _recipes.Length; i++)
        {
            var recipe = _recipes[i];

            var recipeObject = new GameObject(name="Recipe");
            recipeObject.transform.parent = _craftUI.transform;
            
            for (var j = 0; j <= recipe._components.Length; j++)
            {
                var recipeUI = Instantiate(_recipeSlotPrefab, recipeObject.transform, true);
                recipeUI.GetComponent<RectTransform>().localPosition = 
                    new Vector3(j * multiplierUI.x + offsetUI.x, i * multiplierUI.y + offsetUI.y, 0);
                
                var sprite = j == recipe._components.Length ? recipe._item.sprite : recipe._components[j].sprite;
                var amount = j == recipe._components.Length ? recipe._itemCount : recipe._componentsCount[j];

                recipeUI.GetComponent<CraftingSlot>().index = i;
                recipeUI.GetComponent<Button>().enabled = j == recipe._components.Length;
                recipeUI.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                recipeUI.transform.GetChild(1).GetComponent<Text>().text = amount.ToString();

                recipeUI.GetComponent<Outline>().enabled = j == recipe._components.Length;
            }
            
            _recipesUI.Add(recipeObject);
        }
    }

    private bool CheckOven()
    {
        var playerPos = World.player.GetPosition();

        for (int i = (int)playerPos.x - 3; i < playerPos.x + 3; i++)
        {
            for (int j = (int)playerPos.y - 3; j < playerPos.y + 3; j++)
            {
                var block = World.GetBlock(i, j);
                var bgBlock = World.GetBlock(i, j, true);
                
                if (block != null && (block.name == "Oven"|| bgBlock != null && block.name == "Oven"))
                    return true;
            }
        }

        return false;
    }
    
    private void Update()
    {
        var count = 0;
        foreach (var recipeUI in _recipesUI)
        {
            var recipe = _recipes[count];
            var canCraft = recipe.CanCraft(World.player.GetInventory());
            var activeState = canCraft && !recipe.ovenNeed || canCraft && recipe.ovenNeed && CheckOven();
            recipeUI.SetActive(activeState);
            count++;
        }
    }
}
