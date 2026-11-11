using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;
    public int attackDamage = 15;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("AI Ranges")]
    public float detectionRange = 6f; // Tầm nhìn thấy Player
    public float attackRange = 1.5f;  // Tầm vung tay đánh Player
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    private Transform currentTarget;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private bool isAttacking = false;

    [Header("Health UI")]
    public Canvas healthCanvas; // Kéo EnemyHealthCanvas vào đây
    public Image healthFill;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Tự động tìm Player qua Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        currentTarget = pointB;

        if (healthCanvas != null)
        {
            healthCanvas.enabled = false;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- CODE MỚI: KIỂM TRA GIỚI HẠN KHU VỰC ---
        // Lấy tọa độ X của bên trái và bên phải (phòng hờ bạn đặt PointA hoặc PointB lộn xộn)
        float leftBound = Mathf.Min(pointA.position.x, pointB.position.x);
        float rightBound = Mathf.Max(pointA.position.x, pointB.position.x);

        // Kiểm tra xem Player có nằm bên trong khu vực này không
        bool isPlayerInBounds = player.position.x >= leftBound && player.position.x <= rightBound;
        // ------------------------------------------

        if (distanceToPlayer <= attackRange && isPlayerInBounds)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange && isPlayerInBounds)
        {
            // Chỉ rượt đuổi nếu Player vẫn còn nằm trong 2 điểm A và B
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (healthCanvas != null)
        {
            healthCanvas.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1, 1);
        }
    }

    void Patrol()
    {
        if (isAttacking) return;

        anim.SetFloat("Speed", moveSpeed);
        MoveTowards(currentTarget.position, moveSpeed);

        // Nếu đã đi đến mục tiêu, quay đầu đi sang điểm còn lại
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }
    }

    void ChasePlayer()
    {
        if (isAttacking) return;

        anim.SetFloat("Speed", chaseSpeed);
        MoveTowards(player.position, chaseSpeed);
    }

    void MoveTowards(Vector2 targetPos, float speed)
    {
        float direction = targetPos.x - transform.position.x;

        // SỬA DẤU TRỪ THÀNH DẤU NHÂN Ở ĐÂY
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * speed, rb.linearVelocity.y);

        if (direction > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;

            // Gọi hàm gây sát thương sau 0.5s để khớp với lúc hoạt ảnh chém hạ xuống
            Invoke(nameof(DealDamageToPlayer), 0.5f);
        }
    }

    void DealDamageToPlayer()
    {
        if (isDead) return;

        // Kiểm tra lại xem lúc vung dao xuống, Player có né chạy đi chỗ khác chưa
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            // Gọi hàm TakeDamage của PlayerController mà bạn đã cấu hình từ trước
            PlayerController.instance.TakeDamage(attackDamage, gameObject);
        }

        isAttacking = false;
    }

    public void TakeDamage(int damage, GameObject source)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hit");

        // --- CODE CẬP NHẬT THANH MÁU ---
        if (healthFill != null)
        {
            healthFill.fillAmount = (float)currentHealth / maxHealth;
        }

        // Bật hiển thị Canvas thanh máu khi bị đánh mất máu
        if (healthCanvas != null && currentHealth < maxHealth)
        {
            healthCanvas.enabled = true;
        }

        // Lực văng (ĐÃ SỬA THÀNH DẤU NHÂN)
        float knockbackDir = transform.position.x < source.transform.position.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(knockbackDir * 4f, 5f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        if (healthCanvas != null) healthCanvas.enabled = false;

        this.enabled = false;
        Destroy(gameObject, 3f);
    }
}