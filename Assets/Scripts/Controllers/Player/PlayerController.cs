using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public bool isPlayer1 = true;

    [Header("Movement Stats")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float dashSpeed = 8f;

    // Tùy chỉnh độ cao nhảy ở đây (Ví dụ: Giảm từ 12 xuống 8.5f)
    public float jumpForce = 8.5f;

    [Header("Double Jump Settings")]
    public int maxJumps = 2; // Số lần nhảy tối đa (2 = Double Jump)
    private int jumpCount = 0; // Biến đếm số lần đã nhảy

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 originalScale;

    private bool isGrounded;
    private bool isBlocking;
    private bool isDashing;
    private bool isAttacking;

    private KeyCode jumpKey, downKey, leftKey, rightKey;
    private KeyCode attackKey, raseganKey, dashKey;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;
        AssignInputs();
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
            raseganKey = KeyCode.Keypad2;
            dashKey = KeyCode.Keypad3;
        }
    }

    void Update()
    {
        if (anim.GetBool("isDead")) return;

        CheckGroundStatus();
        HandleDefense();

        if (isBlocking) return;

        HandleCombatAndSkills();

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsName("Attack") || stateInfo.IsName("Skill 1") ||
                      stateInfo.IsName("Skill 2") || stateInfo.IsName("Rasegan");

        if (!isAttacking)
        {
            HandleMovement();
            HandleJump();
        }
    }

    void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded; // Lưu trạng thái frame trước

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // Reset số lần nhảy khi chạm đất
        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpCount = 0;
        }
    }

    void HandleJump()
    {
        // Điều kiện nhảy: Bấm phím Nhảy VÀ số lần đã nhảy nhỏ hơn giới hạn tối đa
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps)
        {
            // Reset vận tốc trục Y trước khi áp dụng lực nhảy mới (rất quan trọng cho Double Jump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Áp dụng lực nhảy
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Tăng biến đếm
            jumpCount++;

            // Kích hoạt animation (Nên tạo một trigger nhảy riêng cho bước 2 nếu bạn có hình ảnh nhào lộn)
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
        if (Input.GetKeyDown(raseganKey))
        {
            anim.SetTrigger("Rasegan");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (Input.GetKeyDown(attackKey))
        {
            if (Input.GetKey(downKey))
            {
                anim.SetTrigger("Skill1");
            }
            else if (Input.GetKey(rightKey) || Input.GetKey(leftKey))
            {
                anim.SetTrigger("Skill2");
            }
            else
            {
                anim.SetTrigger("Attack");
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

    public void TakeHit()
    {
        if (!isBlocking)
        {
            anim.SetTrigger("Hit");
        }
    }

    public void Die()
    {
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }
}