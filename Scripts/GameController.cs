using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private bool gameShouldBeStarted = false;
    public bool GameStarted { get; private set; }
    public bool GameOver { get; private set; }

    [Header("Map Generator")]
    [SerializeField] private int ColoredBlockChance;
    [SerializeField] private int SpikeChance;
    [SerializeField] private GameObject[] Blocks;
    [SerializeField] private GameObject[] Spikes;
    [SerializeField] private int minTilesBetweenSpikes;
    [SerializeField] private int maxTilesBetweenSpikes;
    private float xPos = -5.5f;
    private float yPos = -2.85f;
    private bool canSpawnSpike = true;
    private int spawnedSpikesGenerationAgo = 0;

    [Header("UI Management")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject playerJumpButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    private int score;
    private int bestScore;
    private const float ADD_SCORE_TIME = 1.5f;
    private float timeToAddScore = ADD_SCORE_TIME;
    private const string SCORE_KEY = "Score";

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt(SCORE_KEY, 0);
        UpdateScoreTexts();
    }

    private void Update()
    {
        GenerateMap();
        if (GameStarted && !GameOver)
            CountScore();
    }

    // Generating map

    private void GenerateMap()
    {
        var camera = Camera.main;

        if (xPos > camera.transform.position.x + camera.orthographicSize * camera.aspect + 1)
            return;

        int tileIndex = 0;
        bool spawnSpike = false;

        if (Random.Range(0, 101) <= ColoredBlockChance)
            tileIndex = Random.Range(1, 3);
        
        if (Random.Range(0, 101) <= SpikeChance && xPos > 0)
            spawnSpike = true;

        SpawnBlock(tileIndex);

        if (spawnSpike && canSpawnSpike || spawnedSpikesGenerationAgo > maxTilesBetweenSpikes)
        {
            SpawnSpike(tileIndex);
            spawnedSpikesGenerationAgo = 0;
            canSpawnSpike = false;
        }
        
        spawnedSpikesGenerationAgo++;

        if (spawnedSpikesGenerationAgo > minTilesBetweenSpikes)
            canSpawnSpike = true;

        xPos++;
    }

    
    private void SpawnBlock(int index) => Instantiate(Blocks[index], new Vector2(xPos, yPos), Quaternion.identity);
    private void SpawnSpike(int index) => Instantiate(Spikes[index], new Vector2(xPos, yPos + 1), Quaternion.identity);

    // Game modes

    private void LateUpdate()
    {
        if (gameShouldBeStarted)
            GameStarted = true;
    }

    public void ClickInterpretator(string buttonType)
    {
        if (buttonType == "Start")
        {
            gameShouldBeStarted = true;
            startUI.SetActive(false);
            playerJumpButton.SetActive(true);
        }
        else if (buttonType == "Restart")
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void EndGame()
    {
        gameOverUI.SetActive(true);
        playerJumpButton.SetActive(false);
        GameOver = true;
    }

    // Score counting

    private void CountScore()
    {
        if (((timeToAddScore -= Time.deltaTime) * Time.timeScale) <= 0)
        {
            score++;
            if (score > bestScore)
            {
                bestScore = score;
                PlayerPrefs.SetInt(SCORE_KEY, bestScore);
            }

            timeToAddScore = ADD_SCORE_TIME;

            UpdateScoreTexts();
        }
    }

    private void UpdateScoreTexts()
    {
        scoreText.text = $"Score: {score}";
        bestScoreText.text = $"Best: {bestScore}";   
    }
}