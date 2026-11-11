using UnityEngine;

public class MovingPlatformTB : MonoBehaviour
{
    [Header("Cài đặt Bệ thang máy (Lên Xuống)")]
    public float speed = 2f;
    public float moveDistance = 5f; // Khoảng cách bệ sẽ trượt lên/xuống
    public bool startMovingUp = false; // Bệ sẽ đi lên trước?

    private Vector2 startPos;
    private float minY, maxY;
    private int direction;

    void Start()
    {
        startPos = transform.position;
        direction = startMovingUp ? 1 : -1;
        
        // Tính toán giới hạn trên và dưới
        if (startMovingUp)
        {
            minY = startPos.y;
            maxY = startPos.y + moveDistance;
        }
        else
        {
            minY = startPos.y - moveDistance;
            maxY = startPos.y;
        }
    }

    void Update()
    {
        // Tính toán vị trí mới trên trục Y
        float newY = transform.position.y + (direction * speed * Time.deltaTime);

        // Kiểm tra nếu chạm tới giới hạn thì quay đầu
        if (newY >= maxY)
        {
            newY = maxY;
            direction = -1; // Đổi hướng đi xuống
        }
        else if (newY <= minY)
        {
            newY = minY;
            direction = 1; // Đổi hướng đi lên
        }

        // Cập nhật vị trí bệ đất (giữ nguyên trục X)
        transform.position = new Vector2(transform.position.x, newY);
    }
    
    // ĐOẠN CODE NÀY GIÚP NHÂN VẬT ĐỨNG VỮNG TRÊN BỆ ĐẤT KHI DI CHUYỂN
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}