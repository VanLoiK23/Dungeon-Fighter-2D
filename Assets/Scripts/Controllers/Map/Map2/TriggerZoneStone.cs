using UnityEngine;

public class TriggerZoneStone : MonoBehaviour
{
    private RollingStone rollingStoneScript;

    void Start()
    {
        // Tự động tìm script RollingStone ở object cha (Đá lăn) mà không cần kéo thả thủ công
        rollingStoneScript = GetComponentInParent<RollingStone>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone - Stone is rolling!");
            
            if (rollingStoneScript != null)
            {
                rollingStoneScript.ActivateStone();
            }
            
            // Tắt Trigger Zone đi để tránh kích hoạt nhiều lần liên tục
            gameObject.SetActive(false); 
        }
    }
}