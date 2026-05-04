using UnityEngine;

/// <summary>
/// Obstacle that moves back and forth along a path.
/// Use for cars and pedestrians.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingObstacle : MonoBehaviour
{
    public enum MoveAxis { Horizontal, Vertical }

    [Header("Movement")]
    public MoveAxis moveAxis = MoveAxis.Horizontal;
    [Tooltip("Distance to travel from start position")]
    public float moveDistance = 5f;
    [Tooltip("Movement speed")]
    public float moveSpeed = 3f;
    [Tooltip("Pause time at each end")]
    public float pauseTime = 1f;

    [Header("Visuals")]
    public bool flipSpriteOnTurn = true;
    public SpriteRenderer spriteRenderer;

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool movingToEnd = true;
    private bool isPaused;
    private float pauseTimer;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        startPos = rb.position;
        CalculateTarget();
    }

    void FixedUpdate()
    {
        if (isPaused)
        {
            pauseTimer -= Time.fixedDeltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                movingToEnd = !movingToEnd;
                CalculateTarget();
            }
            return;
        }

        Vector2 direction = (targetPos - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Check if reached target
        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            rb.position = targetPos;
            rb.linearVelocity = Vector2.zero;
            isPaused = true;
            pauseTimer = pauseTime;
        }
    }

    void CalculateTarget()
    {
        Vector2 offset = Vector2.zero;
        if (moveAxis == MoveAxis.Horizontal)
            offset = new Vector2(moveDistance * (movingToEnd ? 1f : -1f), 0f);
        else
            offset = new Vector2(0f, moveDistance * (movingToEnd ? 1f : -1f));

        targetPos = startPos + offset;

        if (flipSpriteOnTurn && spriteRenderer != null)
        {
            Vector3 scale = spriteRenderer.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (movingToEnd ? 1f : -1f);
            spriteRenderer.transform.localScale = scale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 s = Application.isPlaying ? startPos : (Vector2)transform.position;
        Vector2 e = s + (moveAxis == MoveAxis.Horizontal ? new Vector2(moveDistance, 0f) : new Vector2(0f, moveDistance));
        Gizmos.DrawLine(s, e);
        Gizmos.DrawWireSphere(s, 0.2f);
        Gizmos.DrawWireSphere(e, 0.2f);
    }
}
