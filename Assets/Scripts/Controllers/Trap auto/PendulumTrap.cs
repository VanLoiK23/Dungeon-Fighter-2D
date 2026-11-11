using UnityEngine;

public class PendulumTrap : MonoBehaviour
{
    [Header("Cài đặt Con lắc")]
    [Tooltip("Tốc độ văng qua lại (số càng lớn lắc càng nhanh)")]
    public float speed = 2f; 
    
    [Tooltip("Góc văng tối đa tính bằng độ (ví dụ: 60 độ)")]
    public float maxAngle = 60f; 

    private void Update()
    {
        // Hàm Mathf.Sin giúp tạo ra một dải số chạy mượt mà từ -1 đến 1 theo thời gian.
        // Nhân với maxAngle sẽ tạo ra góc xoay qua lại từ -60 độ đến 60 độ.
        float currentAngle = maxAngle * Mathf.Sin(Time.time * speed);
        
        // Áp dụng góc xoay này vào trục Z của Điểm treo
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}