
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        SpikeTrapDamage.TryDamage(other.gameObject, gameObject, 0, true);
    }
}
