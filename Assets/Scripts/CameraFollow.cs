using UnityEngine;

/// <summary>
/// Simple smooth camera follow for the player.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Target to follow (usually the player)")]
    public Transform target;

    [Tooltip("How quickly the camera catches up to the target")]
    public float smoothSpeed = 5f;

    [Tooltip("Offset from target position")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
