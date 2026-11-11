using UnityEngine;

public class RollingStone : MonoBehaviour
{
    [Header("Cài đặt tốc độ và lực")]
    public float maxSpeed = 12f;         // Tốc độ lăn tối đa cho phép
    public float knockbackForce = 10f; 
    public int damageAmount = 20;      

    private Rigidbody2D rb;
    private bool isActivated = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero; 
        }
    }

    public void ActivateStone()
    {
        if (!isActivated)
        {
            isActivated = true;
            if (rb != null)
            {
                rb.gravityScale = 2f; // Tăng trọng lực khi kích hoạt
            }
        }
    }

    // Dùng FixedUpdate để xử lý các vấn đề liên quan đến vận tốc vật lý
    void FixedUpdate()
    {
        if (isActivated && rb != null)
        {
            // Giới hạn tốc độ tối đa của viên đá không vượt quá maxSpeed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero; 
                float dirX = Mathf.Sign(collision.transform.position.x - transform.position.x);
                Vector2 knockback = new Vector2(dirX * knockbackForce, knockbackForce * 0.4f);
                playerRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            Debug.Log("Player bị đá đè trúng! HP trừ: " + damageAmount);
        }
    }
}