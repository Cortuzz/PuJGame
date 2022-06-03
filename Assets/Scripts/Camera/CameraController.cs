using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    [Range(0f, 1f)]
    public float smoothTime;

    public Transform playerTransform;
    public void FixedUpdate()
    {
        Vector3 position = GetComponent<Transform>().position;

        position.x = Mathf.Lerp(position.x, playerTransform.position.x, smoothTime);
        position.y = Mathf.Lerp(position.y, playerTransform.position.y, smoothTime);

        GetComponent<Transform>().position = position;
    }
}
