using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <--- CHANGE TO TMPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameOver = false;

    [Header("UI Elements")]
    public TMP_Text scoreText; // <--- CHANGE TO TMP_Text
    public TMP_Text coinText;  // <--- CHANGE TO TMP_Text
    // ... rest of the script

    private float score = 0f;
    private int coins = 0;

    void Awake()
    {
        // This makes sure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // If the game is over, stop counting score
        if (isGameOver) return;

        // Increase score by 10 points every second
        score += Time.deltaTime * 10f;

        // Update the UI text
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }

        if (coinText != null)
        {
            coinText.text = "Coins: " + coins.ToString();
        }
    }

    // This is called by the PlayerController when a coin is collected
    public void AddCoin()
    {
        coins++;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GAME OVER - Final Score: " + Mathf.FloorToInt(score) + " - Coins: " + coins);

        // Wait 2 seconds, then restart the level
        Invoke(nameof(Restart), 2f);
    }

    void Restart()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}