using UnityEngine;

public class MovingPlatformLR : MonoBehaviour
{
    [Header("Cài đặt Bệ di chuyển ngang")]
    public float speed = 2f;
    public float moveDistance = 5f; // Khoảng cách bệ sẽ trượt đi
    public bool startMovingRight = true; // Bệ sẽ chạy sang phải trước?

    private Vector2 startPos;
    private float minX, maxX;
    private int direction;

    void Start()
    {
        startPos = transform.position;
        direction = startMovingRight ? 1 : -1;
        
        // Tính toán giới hạn trái và phải
        if (startMovingRight)
        {
            minX = startPos.x;
            maxX = startPos.x + moveDistance;
        }
        else
        {
            minX = startPos.x - moveDistance;
            maxX = startPos.x;
        }
    }

    void Update()
    {
        // Tính toán vị trí mới trên trục X
        float newX = transform.position.x + (direction * speed * Time.deltaTime);

        // Kiểm tra nếu chạm tới giới hạn thì quay đầu
        if (newX >= maxX)
        {
            newX = maxX;
            direction = -1; // Đổi hướng sang trái
        }
        else if (newX <= minX)
        {
            newX = minX;
            direction = 1; // Đổi hướng sang phải
        }

        // Cập nhật vị trí bệ đất
        transform.position = new Vector2(newX, transform.position.y);
    }
    
    // ĐOẠN CODE NÀY GIÚP NHÂN VẬT ĐỨNG VỮNG TRÊN BỆ ĐẤT KHI DI CHUYỂN
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Làm cho nhân vật trở thành con của bệ đất khi đứng lên
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Xóa quan hệ cha con khi nhân vật nhảy khỏi bệ
            collision.transform.SetParent(null);
        }
    }
}