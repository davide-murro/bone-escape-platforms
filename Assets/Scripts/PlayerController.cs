using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerParts
{
    Head,
    Body,
    Legs
}

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float speed = 5f;
    [SerializeField] float sprintSpeed = 20f;

    [Header("Jump")]
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float jumpEffectDuration = 1f;
    [SerializeField] Collider2D touchingGroundCollider;

    [Header("Die")]
    [SerializeField] Vector2 deathKick = new Vector2(0, 10f);

    [SerializeField] PlayerParts playerPart;

    Vector2 moveInput;
    Animator animator;
    Rigidbody2D rb2d;

    int totalHealthAmount;
    int healthAmount;
    float currentSpeed;
    bool hasHorizontalSpeed = false;
    bool isTouchingGround = false;
    bool isAlive = true;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // destroy this if there is another player
        int numGameSession = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;
        if (numGameSession > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        currentSpeed = speed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameSession gameSession = FindFirstObjectByType<GameSession>();
        DamageReceiver[] receivers = GetComponentsInChildren<DamageReceiver>();

        if (gameSession.savedHealthAmount >= 0)
        {
            // Distribute saved health evenly
            int healthPerReceiver = gameSession.savedHealthAmount / receivers.Length;
            foreach (DamageReceiver receiver in receivers)
            {
                receiver.Heal(-receiver.GetHelth()); // Reset to 0
                receiver.Heal(healthPerReceiver);
            }

            // Reset after applying
            gameSession.savedHealthAmount = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;

        CheckMove();
        CheckFlipSprite();
        CheckTouchGround();
        CheckHealth();
        CheckDead();
    }

    // on move
    void OnMove(InputValue value)
    {
        if (!isAlive) return;

        moveInput = value.Get<Vector2>();
    }

    // on jump
    void OnJump(InputValue value)
    {
        if (!isAlive) return;

        if (value.isPressed && isTouchingGround)
        {
            rb2d.linearVelocity += new Vector2(rb2d.linearVelocity.x, jumpSpeed);
            animator.SetBool("isJumping", true);
            Invoke(nameof(JumpEffectEnd), jumpEffectDuration);
        }
    }

    // on sprint
    void OnSprint(InputValue value)
    {
        if (!isAlive) return;

        if (value.isPressed)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    // get total health
    public int GetTotalHealthAmount()
    {
        return totalHealthAmount;
    }
    // get health amount
    public int GetHealthAmount()
    {
        return healthAmount;
    }
    // add health amount
    public void AddHelth(int amount)
    {
        DamageReceiver receiver = GetComponentInChildren<DamageReceiver>();
        receiver.Heal(amount);
    }

    // check move
    void CheckMove()
    {
        // we have to add interpolation to the move, when it stop don t just stop, but it smoothly stops
        Vector2 velocity = new Vector2(moveInput.x * currentSpeed, rb2d.linearVelocity.y);   // move only in X axis and leave the Y as is
        rb2d.linearVelocity = velocity;

        hasHorizontalSpeed = Mathf.Abs(rb2d.linearVelocity.x) > Mathf.Epsilon; // movement > 0
        animator.SetBool("isWalking", hasHorizontalSpeed);
    }

    // flip the sprite when it changes direction
    void CheckFlipSprite()
    {
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb2d.linearVelocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y);  // I change the sign of localScale.X and keep the same localScale.Y
        }
    }

    // check if is touching the ground
    void CheckTouchGround()
    {
        isTouchingGround = touchingGroundCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Enemy"));
    }

    void JumpEffectEnd()
    {
        animator.SetBool("isJumping", false);
    }

    // check if is alive
    void CheckDead()
    {
        if (healthAmount <= 0)
        {
            // set is dead
            isAlive = false;
            animator.SetTrigger("die");
            rb2d.linearVelocity = deathKick;
            FindFirstObjectByType<GameSession>().ProcessPlayerDeath();
        }
    }

    // take the sum of health of all DamageReceiver inside di game object
    void CheckHealth()
    {
        int totalHealth = 0;
        int health = 0;
        DamageReceiver[] receivers = GetComponentsInChildren<DamageReceiver>();
        foreach (DamageReceiver receiver in receivers)
        {
            totalHealth += receiver.GetTotalHelth();
            health += receiver.GetHelth();
        }
        totalHealthAmount = totalHealth;
        healthAmount = health;
    }
}
