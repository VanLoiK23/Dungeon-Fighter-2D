using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class EnemyAIMap2 : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int attackDamage = 15;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("AI Ranges")]
    public float detectionRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    [Min(0.1f)] public float attackRecoveryDuration = 2.1f;

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Health UI")]
    public Canvas healthCanvas;
    public Image healthFill;

    [Header("Death")]
    [Min(0f)] public float deathDestroyDelay = 2f;

    private int currentHealth;
    private float nextAttackTime;
    private float attackStartedAt;
    private float hitStunUntil;
    private float horizontalDirection;
    private float currentMoveSpeed;
    private Transform currentTarget;
    private Transform player;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isDead;
    private bool isAttacking;
    private bool attackAnimationEntered;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        currentTarget = pointB != null ? pointB : pointA;
        nextAttackTime = Time.time;

        if (healthCanvas == null)
            healthCanvas = GetComponentInChildren<Canvas>(true);

        if (healthFill == null && healthCanvas != null)
        {
            foreach (Image image in healthCanvas.GetComponentsInChildren<Image>(true))
            {
                if (image.type == Image.Type.Filled)
                {
                    healthFill = image;
                    break;
                }
            }
        }

        UpdateHealthUI();
        if (healthCanvas != null)
            healthCanvas.enabled = false;

        FindPlayer();
    }

    private void Update()
    {
        if (isDead || Time.time < hitStunUntil)
            return;

        if (isAttacking)
        {
            StopMovement();

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_1"))
                attackAnimationEntered = true;
            else if (attackAnimationEntered && !anim.IsInTransition(0))
                EndAttack();
            else if (Time.time >= attackStartedAt + Mathf.Max(0.1f, attackRecoveryDuration))
                EndAttack();

            if (isAttacking)
                return;
        }

        if (player == null)
            FindPlayer();

        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isPlayerInBounds = IsPlayerInPatrolBounds();

        if (distanceToPlayer <= attackRange && isPlayerInBounds)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange && isPlayerInBounds)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        if (isDead || Time.time < hitStunUntil)
            return;

        rb.linearVelocity = new Vector2(horizontalDirection * currentMoveSpeed, rb.linearVelocity.y);
    }

    private void FindPlayer()
    {
        if (PlayerController.instance != null)
        {
            playerController = PlayerController.instance;
            player = playerController.transform;
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
            return;

        player = playerObject.transform;
        playerController = playerObject.GetComponentInParent<PlayerController>();
    }

    private bool IsPlayerInPatrolBounds()
    {
        if (pointA == null || pointB == null)
            return true;

        float leftBound = Mathf.Min(pointA.position.x, pointB.position.x);
        float rightBound = Mathf.Max(pointA.position.x, pointB.position.x);
        return player.position.x >= leftBound && player.position.x <= rightBound;
    }

    private void Patrol()
    {
        if (isAttacking || pointA == null || pointB == null)
        {
            StopMovement();
            return;
        }

        if (currentTarget == null)
            currentTarget = pointB != null ? pointB : pointA;

        float direction = currentTarget.position.x - transform.position.x;
        if (Mathf.Abs(direction) < 0.5f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
            direction = currentTarget.position.x - transform.position.x;
        }

        Move(direction, moveSpeed);
    }

    private void ChasePlayer()
    {
        if (isAttacking)
            return;

        Move(player.position.x - transform.position.x, chaseSpeed);
    }

    private void Move(float direction, float speed)
    {
        horizontalDirection = direction > 0f ? 1f : direction < 0f ? -1f : 0f;
        currentMoveSpeed = Mathf.Max(0f, speed);
        anim.SetFloat("Speed", horizontalDirection == 0f ? 0f : currentMoveSpeed);

        if (horizontalDirection != 0f && spriteRenderer != null)
            spriteRenderer.flipX = horizontalDirection < 0f;
    }

    private void StopMovement()
    {
        horizontalDirection = 0f;
        currentMoveSpeed = 0f;
        anim.SetFloat("Speed", 0f);
    }

    private void AttackPlayer()
    {
        StopMovement();

        if (isAttacking || Time.time < nextAttackTime)
            return;

        isAttacking = true;
        attackAnimationEntered = false;
        attackStartedAt = Time.time;
        anim.SetTrigger("Attack");
    }

    public void DealDamageToPlayer()
    {
        if (isDead || !isAttacking || player == null)
            return;

        if (Vector2.Distance(transform.position, player.position) > attackRange)
            return;

        if (playerController == null)
        {
            playerController = player.GetComponentInParent<PlayerController>();
            if (playerController == null)
                playerController = PlayerController.instance;
        }

        if (playerController != null)
            playerController.TakeDamage(attackDamage, gameObject);
    }

    private void EndAttack()
    {
        isAttacking = false;
        nextAttackTime = Time.time + Mathf.Max(0f, attackCooldown);
    }

    public void TakeDamage(int damage, GameObject source)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        anim.SetTrigger("Hit");
        UpdateHealthUI();

        if (healthCanvas != null && currentHealth < maxHealth)
            healthCanvas.enabled = true;

        float knockbackDirection = source != null
            ? (transform.position.x < source.transform.position.x ? -1f : 1f)
            : -Mathf.Sign(transform.localScale.x);
        horizontalDirection = 0f;
        currentMoveSpeed = 0f;
        hitStunUntil = Time.time + 0.15f;
        anim.SetFloat("Speed", 0f);
        rb.linearVelocity = new Vector2(knockbackDirection * 4f, 5f);

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = (float)currentHealth / Mathf.Max(1, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        foreach (Collider2D enemyCollider in GetComponents<Collider2D>())
            enemyCollider.enabled = false;

        if (healthCanvas != null)
            healthCanvas.enabled = false;

        Destroy(gameObject, Mathf.Max(0f, deathDestroyDelay));
    }

}
