using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkeletonKnifeThrow : MonoBehaviour
{
    [Header("Detection and Attack")]
    [SerializeField, Min(0.1f)] private float attackRange = 8f;
    [SerializeField, Min(0f)] private float attackCooldown = 2f;
    [SerializeField, Min(0)] private int damage = 10;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 8f;
    [SerializeField] private bool spriteFacesLeftByDefault = true;

    [Header("Throw Setup")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject knifeProjectilePrefab;
    [SerializeField] private float projectileRotationOffset;

    private Animator animator;
    private SpriteRenderer skeletonRenderer;
    private Sprite idleSprite;
    private Transform player;
    private float nextAttackTime;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        skeletonRenderer = GetComponentInChildren<SpriteRenderer>();
        if (skeletonRenderer != null)
            idleSprite = skeletonRenderer.sprite;

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.enabled = false;
    }

    private void Update()
    {
        if (isAttacking)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!animator.IsInTransition(0)
                && state.IsName("Attack")
                && state.normalizedTime >= 1f)
            {
                EndAttack();
            }

            return;
        }

        if (player == null)
            player = FindPlayer();
        if (player == null)
            return;

        FacePlayer();
        if (Vector2.Distance(transform.position, player.position) <= attackRange
            && Time.time >= nextAttackTime)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.enabled = true;
        animator.Play("Attack", 0, 0f);
    }

    private void EndAttack()
    {
        animator.enabled = false;
        if (skeletonRenderer != null)
            skeletonRenderer.sprite = idleSprite;

        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    public void ThrowKnife()
    {
        if (!isAttacking)
            return;

        if (throwPoint == null)
        {
            Debug.LogError("SkeletonKnifeThrow requires a Throw Point.", this);
            return;
        }

        if (knifeProjectilePrefab == null)
        {
            Debug.LogError("SkeletonKnifeThrow requires a knife projectile prefab.", this);
            return;
        }

        if (player == null)
            player = FindPlayer();
        if (player == null
            || Vector2.Distance(transform.position, player.position) > attackRange)
        {
            return;
        }

        Vector2 spawnPosition = GetThrowPosition();
        GameObject projectileObject = Instantiate(
            knifeProjectilePrefab,
            spawnPosition,
            Quaternion.identity);
        SpriteRenderer projectileRenderer =
            projectileObject.GetComponent<SpriteRenderer>();

        if (projectileRenderer != null && skeletonRenderer != null)
        {
            projectileRenderer.sortingLayerID = skeletonRenderer.sortingLayerID;
            projectileRenderer.sortingOrder = skeletonRenderer.sortingOrder + 1;
        }

        SkeletonKnifeProjectile projectile =
            projectileObject.GetComponent<SkeletonKnifeProjectile>();
        if (projectile == null)
        {
            Debug.LogError(
                "The knife projectile prefab requires SkeletonKnifeProjectile.",
                projectileObject);
            Destroy(projectileObject);
            return;
        }

        Vector2 direction = player != null
            ? (Vector2)(player.position - (Vector3)spawnPosition)
            : GetFacingDirection();
        float facing = GetFacingDirection().x;
        if (Mathf.Abs(direction.x) < 0.01f || direction.x * facing < 0f)
            direction.x = facing * Mathf.Max(Mathf.Abs(direction.x), 0.01f);

        projectile.Launch(
            direction,
            projectileSpeed,
            projectileRotationOffset,
            damage,
            gameObject);
    }

    private Transform FindPlayer()
    {
        if (PlayerController.instance != null)
            return PlayerController.instance.transform;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }

    private void FacePlayer()
    {
        if (skeletonRenderer == null)
            return;

        float horizontalDirection = player.position.x - transform.position.x;
        if (Mathf.Abs(horizontalDirection) < 0.01f)
            return;

        float desiredFacing = horizontalDirection < 0f ? -1f : 1f;
        float rootDirection = transform.lossyScale.x < 0f ? -1f : 1f;
        float defaultFacing = spriteFacesLeftByDefault ? -1f : 1f;
        skeletonRenderer.flipX = (desiredFacing > 0f) != (defaultFacing * rootDirection > 0f);
    }

    private Vector2 GetThrowPosition()
    {
        float facing = GetFacingDirection().x;
        float frontOffset = Mathf.Abs(throwPoint.position.x - transform.position.x);
        return new Vector2(
            transform.position.x + frontOffset * facing,
            throwPoint.position.y);
    }

    private Vector2 GetFacingDirection()
    {
        float scaleDirection = transform.lossyScale.x < 0f ? -1f : 1f;
        float spriteDirection = skeletonRenderer != null && skeletonRenderer.flipX
            ? -1f
            : 1f;
        float defaultFacing = spriteFacesLeftByDefault ? -1f : 1f;

        return Vector2.right * (defaultFacing * scaleDirection * spriteDirection);
    }
}
