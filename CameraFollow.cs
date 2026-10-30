using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 4, -8);
    public float smoothSpeed = 5f;
    public float xFollowSpeed = 3f;

    private float shakeTimer = 0f;
    private float shakeAmount = 0f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        // Smooth X and Z separately
        float newX = Mathf.Lerp(transform.position.x, targetPos.x, xFollowSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, targetPos.y, smoothSpeed * Time.deltaTime);
        float newZ = Mathf.Lerp(transform.position.z, targetPos.z, smoothSpeed * Time.deltaTime);

        Vector3 finalPos = new Vector3(newX, newY, newZ);

        // If the timer is running, add random shake to the camera position
        if (shakeTimer > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
            shakeOffset.z = 0; // Don't shake forward/backward, only up/down/left/right

            finalPos += shakeOffset;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = finalPos;
    }

    // We will call this function from the PlayerController
    public void ShakeCamera(float duration, float amount)
    {
        shakeTimer = duration;
        shakeAmount = amount;
    }
}