using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo đối tượng chứa Rigidbody 2D của gai vào đây (thường là đối tượng cha hoặc chính quả gai)")]
    public Rigidbody2D spikeRigidbody;

    [Header("Settings")]
    [Tooltip("Thời gian chờ (giây) trước khi gai rơi xuống sau khi Player chạm vào vùng")]
    public float fallDelay = 0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        // Kiểm tra nếu đối tượng chạm vào là Player
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("Player entered trigger zone - Spike is falling!");

            if (fallDelay > 0f)
            {
                Invoke(nameof(DropSpike), fallDelay);
            }
            else
            {
                DropSpike();
            }
        }
    }

    private void DropSpike()
    {
        if (spikeRigidbody != null)
        {
            // Chuyển từ Kinematic sang Dynamic để trọng lực kéo gai rơi xuống
            spikeRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}