using UnityEngine;

/// <summary>
/// Keeps the player inside the defined world bounds by clamping position.
/// Attach to the player or a dedicated bounds manager.
/// </summary>
public class WorldBounds : MonoBehaviour
{
    [Tooltip("Minimum world bounds (bottom-left)")]
    public Vector2 minBounds = new Vector2(-20f, -15f);
    [Tooltip("Maximum world bounds (top-right)")]
    public Vector2 maxBounds = new Vector2(20f, 15f);

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f, 0f);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
