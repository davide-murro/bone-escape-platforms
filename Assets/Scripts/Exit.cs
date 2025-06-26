using UnityEngine;

public class Exit : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool isEnemiesAlive;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemiesAlive();
    }

    // on enter (works only with player because I disabled the other interactions
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemiesAlive)
        {
            FindFirstObjectByType<GameSession>().ProcessPlayerWin();
        }
    }

    void CheckEnemiesAlive()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            isEnemiesAlive = false;
            spriteRenderer.color = Color.white;
        }
        else
        {
            isEnemiesAlive = true;
            spriteRenderer.color = Color.gray;
        }
    }
}
