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
        if (laserEffect == null || laserCollider == null)
        {
            Debug.LogError("GunLaserCycle cần gán Laser Effect và Laser Collider.", this);
            enabled = false;
            return;
        }

        if (laserCollider.GetComponent<SpikeTrapDamage>() == null)
        {
            laserCollider.gameObject.AddComponent<SpikeTrapDamage>();
        }

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