using UnityEngine;

public class RotateTrap : MonoBehaviour
{
    [Tooltip("Tốc độ xoay (để số âm nếu muốn xoay cùng chiều kim đồng hồ)")]
    public float rotateSpeed = -360f;

    void Update()
    {
        // Chỉ thực hiện việc xoay hình ảnh quanh trục Z
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime); 
    }
}