using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerLivesTextbox;
    [SerializeField] TextMeshProUGUI playerCoinsTextbox;

    GameSession gameSession;
    GameObject player;
    GameObject enemy;

    // Awake is called once before the Start
    void Awake()
    {
        gameSession = FindFirstObjectByType<GameSession>();
        player = (GameObject.FindWithTag("Player"));
        enemy = (GameObject.FindWithTag("Enemy"));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ProcessGameOver();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // when home next button is clicked
    public void OnHomeClick()
    {
        SceneManager.LoadScene(0);
    }

    // when play again button is clicked
    public void OnPlayAgainClick()
    {
        SceneManager.LoadScene(1);
    }

    void ProcessGameOver()
    {
        if (gameSession)
        {
            int playerLives = gameSession.playerLives;
            int playerCoins = gameSession.playerCoins;
            playerLivesTextbox.text = playerLives.ToString();
            playerCoinsTextbox.text = playerCoins.ToString();

            if (playerLives > 0)
            {
                player.GetComponent<Animator>().SetTrigger("glory");
                enemy.GetComponent<Animator>().SetTrigger("die");
                enemy.GetComponent<SlimeController>().enabled = false;
            }
            else
            {
                player.GetComponent<Animator>().SetTrigger("die");
            }

            gameSession.ResetGameSession();
        }
    }
}
