using UnityEngine;

public class CoinCollect : MonoBehaviour
{
	public AudioClip collectSound;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// 1. Add score
			if (GameManager.Instance != null)
			{
				GameManager.Instance.AddCoin();
			}

			// 2. Play the sound using the Player's Audio Source
			AudioSource playerAudio = other.GetComponent<AudioSource>();
			if (playerAudio != null && collectSound != null)
			{
				playerAudio.PlayOneShot(collectSound, 1.0f);
			}

			// 3. Destroy the coin
			Destroy(gameObject);
		}
	}
}