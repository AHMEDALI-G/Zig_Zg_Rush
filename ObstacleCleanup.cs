using UnityEngine;

public class ObstacleCleanup : MonoBehaviour
{
    public Transform player;
    public float destroyBehindDistance = 20f;

    void Update()
    {
        if (player == null) return;

        // Delete when behind player
        if (transform.position.z < player.position.z - destroyBehindDistance)
        {
            Destroy(gameObject);
        }
    }
}