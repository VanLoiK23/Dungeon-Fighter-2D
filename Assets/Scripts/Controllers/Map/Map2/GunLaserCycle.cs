using System.Collections;
using UnityEngine;

public class GunLaserCycle : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserEffect;
    public BoxCollider2D laserCollider;

    [Header("Time Settings")]
    public float onDuration = 2f;
    public float offDuration = 2.3f;
    public float startDelay = 0f; // <--- Thêm dòng này

    private void Start()
    {
        StartCoroutine(LaserLoop());
    }

    private IEnumerator LaserLoop()
    {
        // Chờ một chút trước khi bắt đầu chu kỳ đầu tiên
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        while (true)
        {
            laserEffect.SetActive(true);
            laserCollider.enabled = true;

            yield return new WaitForSeconds(onDuration);

            laserCollider.enabled = false;
            laserEffect.SetActive(false);

            yield return new WaitForSeconds(offDuration);
        }
    }
}