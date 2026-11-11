
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã chạm vào lửa!");

            // Sau này có thể trừ máu tại đây
        }
    }
}
