
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingRock : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 previousPosition;

    private Transform playerOnRock;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = rb.position;
        previousPosition = rb.position;
    }

    private void FixedUpdate()
    {
        float x = Mathf.PingPong(
            Time.fixedTime * moveSpeed,
            moveDistance
        );

        Vector2 newPosition = new Vector2(
            startPosition.x + x - moveDistance / 2f,
            startPosition.y
        );

        Vector2 movement = newPosition - previousPosition;

        rb.MovePosition(newPosition);

        // Cho Player di chuyển theo tảng đá
        if (playerOnRock != null)
        {
            Rigidbody2D playerRb =
                playerOnRock.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                playerRb.position += movement;
            }
        }

        previousPosition = newPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                playerOnRock = collision.transform;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform == playerOnRock)
        {
            playerOnRock = null;
        }
    }
}
