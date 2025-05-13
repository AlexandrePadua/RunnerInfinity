using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Canvas gameOverCanvas;

    private float playerDistance = 0f;
    private float totalScore = 0f;
    private float score = 0f;

    private bool gameOver = false;

    public float difficultyMultiplier = 1f;

    private static GameController instance;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("GameController");
                instance = obj.AddComponent<GameController>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        gameOverCanvas.enabled = false;
    }

    void Start()
    {
        playerDistance = 0f;
        distanceText.text = $"{playerDistance:F0}m";
        scoreText.text = $"{totalScore:F0} pts";

        difficultyMultiplier = 1f;
        InvokeRepeating(nameof(IncreaseDifficulty), 15f, 10f);

        restartButton.onClick.AddListener(RestartLevel);
    }

    private void OnEnable()
    {
        PlayerController.onPlayerDeath += GameOver;
    }

    private void OnDisable()
    {
        PlayerController.onPlayerDeath -= GameOver;
    }
    void GameOver()
    {
        gameOver = true;
        gameOverCanvas.enabled = true;
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (gameOver) return;

        playerDistance += Time.deltaTime * (5f * difficultyMultiplier);
        totalScore = playerDistance + score;

        distanceText.text = $"{playerDistance:F0}m";
        scoreText.text = $"{totalScore:F0}pts";
    }

    public void AddScore(float scoreAdded)
    {
        score += scoreAdded;
    }

    private void IncreaseDifficulty()
    {
        difficultyMultiplier += 0.1f;
        Debug.Log(difficultyMultiplier);
    }
}