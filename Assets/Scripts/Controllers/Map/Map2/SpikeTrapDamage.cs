
using UnityEngine;

public class SpikeTrapDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [Min(0)]
    public int damageAmount;

    private static readonly System.Collections.Generic.Dictionary<PlayerController, float> nextDamageTime =
        new System.Collections.Generic.Dictionary<PlayerController, float>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.gameObject, gameObject, damageAmount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other.gameObject, gameObject, damageAmount, true);
    }

    public static bool TryDamage(
        GameObject target,
        GameObject damageSource,
        int damage = 0,
        bool isTrigger = false)
    {
        if (target == null)
            return false;

        PlayerController player = target.GetComponentInParent<PlayerController>();
        if (player == null)
            return false;

        if (isTrigger && target.CompareTag("Player")
            && damageSource != null && damageSource.tag == "Trap")
        {
            SetDamageCooldown(player);
            return false;
        }

        float currentTime = Time.time;
        if (nextDamageTime.TryGetValue(player, out float readyTime)
            && currentTime < readyTime)
        {
            return false;
        }

        int resolvedDamage = damage > 0 ? damage : player.trapDamage;
        if (resolvedDamage <= 0)
            return false;

        SetDamageCooldown(player);
        player.TakeDamage(resolvedDamage, damageSource);
        return true;
    }

    private static void SetDamageCooldown(PlayerController player)
    {
        float cooldown = Mathf.Max(player.invulnerableTime, 0.05f);
        nextDamageTime[player] = Time.time + cooldown;
    }
}
