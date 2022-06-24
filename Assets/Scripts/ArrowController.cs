using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        var angle = Quaternion.LookRotation(Vector3.forward, _rb.velocity).eulerAngles.z;
        transform.localRotation = Quaternion.Euler(0, 0, angle - 20);
    }
}
