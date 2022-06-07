using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject prefab;
    public AnimAtlas animations;
    public int count = -1;
    private void FixedUpdate()
    {
        count++;
        if (count == 0)
            SpawnExecutor();
        //executioner.SetAnimator(animations.executioner);
        //executioner.Spawn(World.width / 2, World.GetHeightAt(World.width / 2) + 15);
    }

    private void SpawnExecutor()
    {
        Instantiate(prefab, transform, false);

        //script.Spawn(World.width / 2, World.GetHeightAt(World.width / 2));
    }
}
