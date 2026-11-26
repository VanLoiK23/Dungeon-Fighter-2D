using UnityEngine;

public class LavaController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void DamagePlayer(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        SpikeTrapDamage.TryDamage(other.gameObject, gameObject, isTrigger: true);
    }
}
