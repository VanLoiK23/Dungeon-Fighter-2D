using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 20;
    public LayerMask enemyLayer;
    
    // Thẻ này giúp biến canDamage hiện ra ngoài bảng Inspector để Record được
    [SerializeField] private bool canDamage = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDamage && ((1 << collision.gameObject.layer & enemyLayer) != 0))
        {
            EnemyAIMap2 map2Enemy = collision.GetComponentInParent<EnemyAIMap2>();
            if (map2Enemy != null)
            {
                map2Enemy.TakeDamage(damage, transform.parent.gameObject);
                canDamage = false;
                return;
            }

            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.parent.gameObject);
                canDamage = false; 
            }
        }
    }
}