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
    private void Start()
    {
        _recipesUI = new List<GameObject>();
        for (var i = 0; i < _recipes.Length; i++)
        {
            var recipeObject = new GameObject(name="Recipe");
            var recipe = _recipes[i];
            recipeObject.transform.parent = _craftUI.transform;
            
            for (var j = 0; j <= recipe._components.Length; j++)
            {
                var recipeUI = Instantiate(_recipeSlotPrefab, recipeObject.transform, true);
                recipeUI.GetComponent<RectTransform>().localPosition = 
                    new Vector3(j * multiplierUI.x + offsetUI.x, i * multiplierUI.y + offsetUI.y, 0);
                
                var sprite = j == recipe._components.Length ? recipe._item.sprite : recipe._components[j].sprite;
                recipeUI.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                
                _recipesUI.Add(recipeUI);
            }
        }
    }
    
    private void Update()
    {
        foreach (var recipe in _recipes)
        {
            
        }
    }
}
