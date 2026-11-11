using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [Header("Cài đặt Lưỡi cưa")]
    [Tooltip("Tốc độ di chuyển")]
    public float speed = 3f;
    
    [Tooltip("Khoảng cách di chuyển (bao nhiêu ô/đơn vị)")]
    public float moveDistance = 5f;

    private Vector3 startPos;

    void Start()
    {
        // Lưu lại vị trí ban đầu của lưỡi cưa khi game bắt đầu
        startPos = transform.position;
    }

    void Update()
    {
        // Tính toán vị trí mới
        // Mathf.PingPong sẽ trả về giá trị chạy từ 0 đến moveDistance rồi lùi về 0 liên tục
        float newX = startPos.x + Mathf.PingPong(Time.time * speed, moveDistance);
        
        // Cập nhật vị trí cho lưỡi cưa (chỉ thay đổi trục X, giữ nguyên Y và Z)
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        // // (Tùy chọn) Làm cho lưỡi cưa tự xoay tròn cho đẹp mắt
        // transform.Rotate(0, 0, 360 * Time.deltaTime); 
    }
}