using UnityEngine;
using UnityEngine.UI; // Bắt buộc phải có để điều khiển Thanh Máu

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public bool isPlayer1 = true;

    public static PlayerController instance;

    [Header("Health & UI")]
    public int maxHealth = 100;
    private int currentHealth;
    public Image healthBarFill;

    [Header("Damage Settings")]
    public int trapDamage = 20; // Số máu bị trừ khi đụng bẫy
    public float invulnerableTime = 1f; // Thời gian bất tử chớp nháy sau khi bị thương
    private float invulTimer = 0f;

    private float knockbackTimer = 0f;

    [Header("Combo System (Video Method)")]
    public bool canReceiveInput = true;
    public bool inputReceived = false;

    [Header("Movement Stats")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float dashSpeed = 8f;
    public float jumpForce = 8.5f;

    [Header("Double Jump Settings")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 originalScale;

    private SpriteRenderer spriteRend;

    private bool isGrounded;
    private bool isBlocking;
    private bool isDashing;
    private bool isAttacking;

    private KeyCode jumpKey, downKey, leftKey, rightKey;
    private KeyCode attackKey, skill1Key, skill2Key, raseganKey, dashKey;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;
        AssignInputs();

        // Khởi tạo máu
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void AssignInputs()
    {
        if (isPlayer1)
        {
            jumpKey = KeyCode.Space;
            downKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            attackKey = KeyCode.J;
            skill1Key = KeyCode.U;
            skill2Key = KeyCode.I;
            raseganKey = KeyCode.K;
            dashKey = KeyCode.L;
        }
        else
        {
            jumpKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            attackKey = KeyCode.Keypad1;
            skill1Key = KeyCode.Keypad4;
            skill2Key = KeyCode.Keypad5;
            raseganKey = KeyCode.Keypad2;
            dashKey = KeyCode.Keypad3;
        }
    }

    void Update()
    {
        if (anim.GetBool("isDead")) return;

        // Giảm thời gian đếm ngược
        if (invulTimer > 0) invulTimer -= Time.deltaTime;
        if (knockbackTimer > 0) knockbackTimer -= Time.deltaTime;

        CheckGroundStatus();
        HandleDefense();

        if (isBlocking) return;

        // KHÓA ĐIỀU KHIỂN: Nếu đang bị văng, ngắt toàn bộ code bên dưới để vật lý tự lo
        if (knockbackTimer > 0) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsTag("Attack") || stateInfo.IsName("Skill 1") ||
                      stateInfo.IsName("Skill 2") || stateInfo.IsName("Rasegan");

        if (!isAttacking)
        {
            HandleCombatAndSkills();
            HandleMovement();
            HandleJump();
        }
        else
        {
            float smoothDeceleration = Mathf.Lerp(rb.linearVelocity.x, 0f, 10f * Time.deltaTime);
            rb.linearVelocity = new Vector2(smoothDeceleration, rb.linearVelocity.y);

            if (Input.GetKeyDown(attackKey) && canReceiveInput)
            {
                inputReceived = true;
                canReceiveInput = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap") && invulTimer <= 0f)
        {
            TakeDamage(trapDamage, collision.gameObject);
        }
    }

    public void TakeDamage(int damage, GameObject damageSource = null)
    {

        // Nếu nguồn sát thương là chính bản thân mình (hoặc con cái của mình), thì từ chối nhận sát thương!
        if (damageSource != null && (damageSource == gameObject || damageSource.transform.IsChildOf(transform)))
        {
            return;
        }

        if (anim.GetBool("isDead")) return;
        if (isBlocking) return;

        currentHealth -= damage;
        invulTimer = invulnerableTime;
        UpdateHealthUI();

        StartCoroutine(FlashRoutine());
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");

            // CẤP QUYỀN CHO VẬT LÝ HOẠT ĐỘNG TRONG 0.3 GIÂY
            knockbackTimer = 0.3f;

            // --- XỬ LÝ HƯỚNG VĂNG THỰC TẾ ---
            if (damageSource != null)
            {
                if (damageSource.name.ToLower().Contains("spike"))
                {
                    rb.linearVelocity = new Vector2(0f, 9f);
                }
                else
                {
                    float knockbackDir = transform.position.x < damageSource.transform.position.x ? -1f : 1f;
                    rb.linearVelocity = new Vector2(knockbackDir * 5f, 6f);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3f, 5f);
            }
        }
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        // Vòng lặp chạy chừng nào thời gian bất tử (invulTimer) vẫn còn
        while (invulTimer > 0f)
        {
            // Đổi màu nhân vật thành Đỏ (pha thêm chút trong suốt)
            spriteRend.color = new Color(1f, 0f, 0f, 0.7f);
            // Đợi 0.1 giây
            yield return new WaitForSeconds(0.1f);

            // Trả lại màu gốc của nhân vật (Màu Trắng trong Unity nghĩa là giữ nguyên màu ảnh gốc)
            spriteRend.color = Color.white;
            // Đợi 0.1 giây
            yield return new WaitForSeconds(0.1f);
        }

        // Đảm bảo chắc chắn nhân vật về lại màu bình thường khi vòng lặp kết thúc
        spriteRend.color = Color.white;
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            // Fill Amount tính theo tỷ lệ từ 0 đến 1
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpCount = 0;
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            anim.SetTrigger("JumpTrigger");
        }
    }

    void HandleDefense()
    {
        if (Input.GetKey(downKey) && isGrounded)
        {
            isBlocking = true;
            anim.SetBool("isBlocking", true);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            isBlocking = false;
            anim.SetBool("isBlocking", false);
        }
    }

    void HandleCombatAndSkills()
    {
        if (Input.GetKeyDown(skill1Key))
        {
            anim.SetTrigger("Skill1");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (Input.GetKeyDown(skill2Key))
        {
            anim.SetTrigger("Skill2");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // if (Input.GetKeyDown(raseganKey))
        // {
        //     anim.SetTrigger("Rasegan");
        //     rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        //     return;
        // }

        if (Input.GetKeyDown(attackKey))
        {
            // Bấm nhát đầu tiên (khi đang không đánh đòn nào)
            if (!isAttacking)
            {
                anim.SetTrigger("AttackOne");
            }
            // Bấm nhát tiếp theo (để nối combo)
            else if (canReceiveInput)
            {
                inputReceived = true;
                canReceiveInput = false;
            }
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void HandleMovement()
    {
        float moveDir = 0f;
        if (Input.GetKey(leftKey)) moveDir = -1f;
        if (Input.GetKey(rightKey)) moveDir = 1f;

        isDashing = Input.GetKey(dashKey);

        float currentSpeed = 0f;
        if (moveDir != 0)
        {
            currentSpeed = isDashing ? dashSpeed : runSpeed;
            transform.localScale = new Vector3(Mathf.Sign(moveDir) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        rb.linearVelocity = new Vector2(moveDir * currentSpeed, rb.linearVelocity.y);
        anim.SetFloat("Speed", currentSpeed);
    }

    // GỌI BẰNG ANIMATION EVENT: Mở cửa sổ cho phép người chơi bấm phím đòn tiếp theo
    public void OpenInputWindow()
    {
        canReceiveInput = true;
        inputReceived = false;
    }

    // GỌI BẰNG ANIMATION EVENT: Kiểm tra xem người chơi có bấm phím kịp không
    // Hàm này gọi ở gần cuối hoạt ảnh Attack1
    public void CheckAttack2()
    {
        if (inputReceived)
        {
            anim.SetTrigger("AttackTwo"); // Kích hoạt mũi tên nối sang Attack2
            inputReceived = false; // Reset lại
            canReceiveInput = false;
        }
    }

    // Hàm này gọi ở gần cuối hoạt ảnh Attack2
    public void CheckAttack3()
    {
        if (inputReceived)
        {
            anim.SetTrigger("AttackThree"); // Kích hoạt mũi tên nối sang Attack3
            inputReceived = false;
            canReceiveInput = false;
        }
    }

    public void Die()
    {
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; // Tắt script 

        Destroy(gameObject, 2f);
    }
}