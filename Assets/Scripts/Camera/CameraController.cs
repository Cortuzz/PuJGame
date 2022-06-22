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

        var position1 = playerTransform.position;
        
        position.x = Mathf.Lerp(position.x, position1.x, smoothTime);
        position.y = Mathf.Lerp(position.y, position1.y, smoothTime);

        GetComponent<Transform>().position = position;
    }
}
