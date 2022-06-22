using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private int _heartsCount;
    [SerializeField] private int _heartHP;
    [SerializeField] private Sprite[] _sprites;
    private GameObject[] _hearts;

    void Start()
    {
        _hearts = new GameObject[_heartsCount];

        for (int i = 0; i < _heartsCount; i++)
        {
            _hearts[i] = transform.GetChild(i).gameObject;
        }
    }

    public void UpdateUI(int hp)
    {
        int i;
        
        for (i = 0; i < _heartsCount; i++)
        {
            hp -= _heartHP;
        
            if (hp < 0)
            {
                hp += _heartHP; 
                break;
            }
            
            _hearts[i].GetComponent<Image>().sprite = _sprites.Last();
        }

        if (hp > 0)
        {
            int formula = hp / (_heartHP / (_sprites.Length - 2)) + 1; 
            
            print(formula);
            _hearts[i].GetComponent<Image>().sprite = _sprites[formula];
            i++;
        }
        
        
        for (; i < _heartsCount; i++)
        {
            _hearts[i].GetComponent<Image>().sprite = _sprites.First();
        }
    }
}