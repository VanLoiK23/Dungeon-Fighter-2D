using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class SkeletonKnifeProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float lifetime = 5f;
    [SerializeField, Min(0.01f)] private float hitRadius = 0.15f;
    [SerializeField] private float spinSpeed = 720f;

    private Rigidbody2D body;
    private GameObject owner;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.angularVelocity = 0f;

        CircleCollider2D hitCollider = GetComponent<CircleCollider2D>();
        hitCollider.isTrigger = true;
        hitCollider.radius = Mathf.Max(0.01f, hitRadius);

        Destroy(gameObject, Mathf.Max(0.1f, lifetime));
    }

    public void Launch(
        Vector2 direction,
        float speed,
        float rotationOffset,
        int projectileDamage,
        GameObject projectileOwner)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            Debug.LogError("Skeleton knife projectile requires a non-zero direction.", this);
            Destroy(gameObject);
            return;
        }

        owner = projectileOwner;
        damage = Mathf.Max(0, projectileDamage);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        body.linearVelocity = direction * Mathf.Max(0.1f, speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || IsOwner(other.transform))
            return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            hasHit = true;
            player.TakeDamage(damage, owner);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        body.MoveRotation(body.rotation + spinSpeed * Time.fixedDeltaTime);
    }

    private bool IsOwner(Transform other)
    {
        return owner != null
            && (other == owner.transform || other.IsChildOf(owner.transform));
    }
}
