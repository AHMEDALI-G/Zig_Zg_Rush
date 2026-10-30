using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
	public float slideDuration = 0.8f;

	private CharacterController controller;
	private float originalHeight;
	private Vector3 originalCenter;
	private bool isSliding = false;
	private float slideTimer = 0f;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		originalHeight = controller.height;
		originalCenter = controller.center;
	}

	void Update()
	{
		if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

		if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isSliding)
		{
			StartSlide();
		}

		if (isSliding)
		{
			slideTimer -= Time.deltaTime;
			if (slideTimer <= 0f) EndSlide();
		}
	}

	void StartSlide()
	{
		isSliding = true;
		slideTimer = slideDuration;

		// Shrink the collider to 1 meter high
		controller.height = 1f;

		// Push the center down so the player is basically crawling
		controller.center = new Vector3(0, 0.5f, 0);

		// Squash the blue capsule visually
		transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);
	}

	void EndSlide()
	{
		isSliding = false;

		// Restore normal size
		controller.height = originalHeight;
		controller.center = originalCenter;
		transform.localScale = Vector3.one;
	}
}