using UnityEngine;

public class BlockDestroyer : MonoBehaviour
{
    private void Update()
    {
        var camera = Camera.main;

        if (transform.position.x < camera.transform.position.x - camera.orthographicSize * camera.aspect - 1)
            Destroy(gameObject);
    }
}
