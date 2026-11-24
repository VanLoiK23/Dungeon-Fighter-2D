using UnityEngine;

public class FallingSpikeTrap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerController>() == null)
            return;

        SpikeTrapDamage.TryDamage(collision.gameObject, gameObject);
    }
}
