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
    
    private void Update()
    {
        var count = 0;
        foreach (var recipeUI in _recipesUI)
        {
            recipeUI.SetActive(_recipes[count].CanCraft(World.player.GetInventory()));
            count++;
        }
    }
}
