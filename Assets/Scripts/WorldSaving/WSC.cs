using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldSaving;

public class WSC : MonoBehaviour
{
    // Start is called before the first frame update
    public void SaveWorld()
    {
        WorldSavingController.SaveToFile();
    }
    
    public void OpenWorld()
    {
        WorldSavingController.LoadFromFile();
    }
}
