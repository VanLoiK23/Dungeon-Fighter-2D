
using System.Collections;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    [Header("Movement")]
    public float moveHeight = 2f;

    [Header("Speed Settings")]
    public float moveUpDuration = 2f;
    public float moveDownDuration = 0.3f;

    [Header("Pause Settings")]
    public float pauseAtTop = 0.5f;
    public float pauseAtBottom = 0.5f;

    private Vector3 startPosition;
    private Vector3 topPosition;

    private void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * moveHeight;

        StartCoroutine(MoveSpike());
    }

    private IEnumerator MoveSpike()
    {
        while (true)
        {
            // Đi lên chậm
            yield return MoveTo(topPosition, moveUpDuration);

            // Dừng trên cao
            yield return new WaitForSeconds(pauseAtTop);

            // Đập xuống nhanh
            yield return MoveTo(startPosition, moveDownDuration);

            // Dừng dưới đất
            yield return new WaitForSeconds(pauseAtBottom);
        }
    }

    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 currentPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            transform.position = Vector3.Lerp(
                currentPosition,
                target,
                t
            );

            yield return null;
        }

        transform.position = target;
    }
}
