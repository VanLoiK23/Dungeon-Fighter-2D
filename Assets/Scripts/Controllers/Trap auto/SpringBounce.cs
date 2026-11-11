using UnityEngine;

public class SpringBounce : MonoBehaviour
{
    [Header("Cài đặt Lò xo")]
    [Tooltip("Lực đẩy hất người chơi lên trên")]
    public float bounceForce = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng chạm vào lò xo có phải là Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null)
            {
                // Xóa bỏ lực rơi hiện tại của người chơi (nếu có) để lực đẩy luôn chuẩn xác
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
                
                // Cung cấp lực đẩy hất lên trời
                playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                
                // (Tùy chọn) Nếu lò xo của bạn có Animation lún xuống rồi bật lên, 
                // bạn có thể gọi nó ở đây, ví dụ: GetComponent<Animator>().SetTrigger("Bounce");
            }
        }
    }
}