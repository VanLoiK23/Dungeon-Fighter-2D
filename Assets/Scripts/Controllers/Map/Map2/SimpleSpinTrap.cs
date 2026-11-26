
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleSpinTrap : MonoBehaviour
{
    [Header("Tốc độ xoay")]
    public float rotationSpeed = 360f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        float nextAngle = rb.rotation
            + rotationSpeed * Time.fixedDeltaTime;

        rb.MoveRotation(nextAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpikeTrapDamage.TryDamage(collision.gameObject, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpikeTrapDamage.TryDamage(other.gameObject, gameObject, 0, true);
    }
}
