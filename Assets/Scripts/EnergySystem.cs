using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{ [SerializeField] private int _energyCount;
    [SerializeField] private int _lightingEnergy;
    [SerializeField] private Sprite[] _sprites;
    private GameObject[] _hearts;

    void Start()
    {
        _hearts = new GameObject[_energyCount];

        for (int i = 0; i < _energyCount; i++)
        {
            _hearts[i] = transform.GetChild(i).gameObject;
        }
    }

    public void UpdateUI(int hp)
    {
        int i;
        
        for (i = 0; i < _energyCount; i++)
        {
            hp -= _lightingEnergy;
        
            if (hp < 0)
            {
                hp += _lightingEnergy; 
                break;
            }
            
            _hearts[i].GetComponent<Image>().sprite = _sprites.Last();
        }

        if (hp > 0 && i < _energyCount)
        {
            var formula = Mathf.RoundToInt(hp / ((float)_lightingEnergy / (_sprites.Length - 2)) + 1);
            _hearts[i].GetComponent<Image>().sprite = _sprites[formula];
            i++;
        }
        
        
        for (; i < _energyCount; i++)
        {
            _hearts[i].GetComponent<Image>().sprite = _sprites.First();
        }
    }
}
