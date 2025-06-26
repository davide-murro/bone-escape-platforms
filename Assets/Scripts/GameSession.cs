using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] public int playerLives = 3;
    [SerializeField] public int playerCoins = 0;
    [SerializeField] float levelLoadDelay = 1f;

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isWinning = false;

    TextMeshProUGUI playerLivesTextbox;
    Slider playerLifeSlider;
    TextMeshProUGUI playerCoinsTextbox;

    public int savedHealthAmount = -1;  // not set by default

    // Awake is called once before the Start
    void Awake()
    {
        // we are going to put a GameSession in every scene.. i check if already exists from the scene before, if doesn t exist  i add the class to the DontDestroyOnLoad.. if exists i destroy the current class and keep the old one
        int numGameSession = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;
        if (numGameSession > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        // player status
        playerLivesTextbox = GameObject.FindWithTag("LivesTextbox").GetComponent<TextMeshProUGUI>();
        playerLifeSlider = GameObject.FindWithTag("LifeSlider").GetComponent<Slider>();
        playerCoinsTextbox = GameObject.FindWithTag("CoinsTextbox").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    // update UI
    void UpdateUI()
    {
        playerLivesTextbox.text = playerLives.ToString();
        playerLifeSlider.maxValue = FindFirstObjectByType<PlayerController>().GetTotalHealthAmount();
        playerLifeSlider.value = FindFirstObjectByType<PlayerController>().GetHealthAmount();
        playerCoinsTextbox.text = playerCoins.ToString();
    }

    // check if the player is dead
    public void ProcessPlayerDeath()
    {
        if (playerLives > 0)
        {
            // set value
            playerLives--;
            Invoke(nameof(ResetLevel), levelLoadDelay);
        }
        else
        {
            isDead = true;
            Invoke(nameof(GameOver), levelLoadDelay);
        }

        //Debug.Log("lives: " + playerLives);
    }

    // check if the player is dead
    public void ProcessPlayerWin()
    {
        if (isWinning) return;

        isWinning = true;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Invoke(nameof(LoadNextLevel), levelLoadDelay);
        }

        savedHealthAmount = FindFirstObjectByType<PlayerController>().GetHealthAmount();
    }

    // load next level
    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
        FindFirstObjectByType<SceneSession>().ResetSceneSession();
        isWinning = false;
    }

    // take one life and update view
    void ResetLevel()
    {
        // load scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        // position to start
        // remove animations triggers
        // set life 100%
        // set the camera
    }

    // when the game finish
    void GameOver()
    {
        FindFirstObjectByType<SceneSession>().ResetSceneSession();
        SceneManager.LoadScene("Game Over");
    }

    // reset game if you lose
    public void ResetGameSession()
    {
        Destroy(gameObject);
    }

    // add coin
    public void AddCoins(int amount)
    {
        playerCoins += amount;
    }

    // add lives
    public void AddLives(int amount)
    {
        playerLives += amount;
    }
}
