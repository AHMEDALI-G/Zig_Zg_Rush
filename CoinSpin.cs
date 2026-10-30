using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    public float spinSpeed = 150f;
    public float floatSpeed = 3f;
    public float floatHeight = 0.2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Spin the coin vertically around the Y axis
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.World);

        // Make it float up and down gently
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}