using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftController : MonoBehaviour
{
    [SerializeField] private GameObject _craftUI;
    [SerializeField] private GameObject _recipeSlotPrefab;
    [SerializeField] private Recipe[] _recipes;
    private List<GameObject> _recipesUI;
    private void Start()
    {
        _recipesUI = new List<GameObject>();
        foreach (var recipe in _recipes)
        {
            var recipeUI = Instantiate(_recipeSlotPrefab, _craftUI.transform, true);
            _recipesUI.Add(recipeUI);
        }
    }
    
    private void Update()
    {
        foreach (var recipe in _recipes)
        {
            
        }
    }
}
