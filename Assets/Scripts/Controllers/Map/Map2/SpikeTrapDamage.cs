
using UnityEngine;

public class SpikeTrapDamage : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player đã chạm vào gai!");
        }
    }
}
