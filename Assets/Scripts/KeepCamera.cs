using UnityEngine;

public class PersistentCamera : MonoBehaviour
{
    private static PersistentCamera instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents destruction on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate cameras
        }
    }
}
