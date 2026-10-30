using UnityEngine;

public class DrumRoller : MonoBehaviour
{
	public float rollSpeed = 8f;
	public float rotationSpeed = 500f;

	void Update()
	{
		// Move the drum towards the player
		transform.Translate(Vector3.back * rollSpeed * Time.deltaTime, Space.World);

		// Spin it on the X axis so it rolls forward like a barrel
		transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
	}
}