using UnityEngine;

public class RotateBullet : MonoBehaviour
{
    public float rotationSpeed = 1100f;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
