
using UnityEngine;

public class ButtonMechanism : MonoBehaviour
{
    [Header("Objects")]
    public Rigidbody2D fallingRock;
    public Rigidbody2D risingWood;

    [Header("Rock Settings")]
    public float rockGravity = 1.5f;

    [Header("Wood Settings")]
    public float riseDistance = 4f;
    public float riseSpeed = 3f;

    private bool isActivated;
    private bool isWoodMoving;

    private Vector2 woodTargetPosition;

    private void Start()
    {
        if (risingWood != null)
        {
            woodTargetPosition = risingWood.position
                + Vector2.up * riseDistance;
        }
    }

    // Player bước vào vùng kích hoạt
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated)
            return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;

            Debug.Log("Player chạm nút - Đá bắt đầu rơi!");

            fallingRock.bodyType = RigidbodyType2D.Dynamic;
            fallingRock.gravityScale = rockGravity;
            fallingRock.WakeUp();
        }
    }

    // Đá đè lên Button
    public void ActivateWood()
    {
        if (isWoodMoving)
            return;

        Debug.Log("Đá đã đè Button - Tấm gỗ bắt đầu bay!");

        isWoodMoving = true;
    }

    private void FixedUpdate()
    {
        if (!isWoodMoving || risingWood == null)
            return;

        Vector2 nextPosition = Vector2.MoveTowards(
            risingWood.position,
            woodTargetPosition,
            riseSpeed * Time.fixedDeltaTime
        );

        risingWood.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, woodTargetPosition) < 0.02f)
        {
            risingWood.position = woodTargetPosition;
            isWoodMoving = false;
        }
    }
}
